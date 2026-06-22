using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;
using ResourceryPlatformWorkflow.Administration.EntityFrameworkCore;
using ResourceryPlatformWorkflow.Auth;
using ResourceryPlatformWorkflow.IdentityService.EntityFrameworkCore;
using ResourceryPlatformWorkflow.Middleware;
using ResourceryPlatformWorkflow.MultiTenancy;
using ResourceryPlatformWorkflow.SaaS.EntityFrameworkCore;
using ResourceryPlatformWorkflow.Workflow;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Auditing;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.DistributedLocking;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using Volo.Abp.UI.Navigation.Urls;

namespace ResourceryPlatformWorkflow;


[DependsOn(typeof(AbpAccountApplicationModule))]
[DependsOn(typeof(AbpAccountHttpApiModule))]
[DependsOn(typeof(AbpAccountWebOpenIddictModule))]
[DependsOn(typeof(AbpAspNetCoreMvcUiLeptonXLiteThemeModule))]
[DependsOn(typeof(AbpAspNetCoreSerilogModule))]
[DependsOn(typeof(AbpAutofacModule))]
[DependsOn(typeof(AbpCachingStackExchangeRedisModule))]
[DependsOn(typeof(AbpDistributedLockingModule))]
[DependsOn(typeof(AbpEntityFrameworkCoreSqlServerModule))]
[DependsOn(typeof(AdministrationEntityFrameworkCoreModule))]
[DependsOn(typeof(IdentityServiceEntityFrameworkCoreModule))]
[DependsOn(typeof(SaaSEntityFrameworkCoreModule))]
[DependsOn(typeof(ResourceryPlatformWorkflowMicroserviceModule))]
[DependsOn(typeof(ResourceryPlatformWorkflowServiceDefaultsModule))]
[DependsOn(typeof(WorkflowDomainSharedModule))]
public class ResourceryPlatformWorkflowAuthServerModule : AbpModule
{
    private static readonly Lazy<Dictionary<string, string>> DotEnvValues = new(LoadDotEnvValues);

    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        AppContext.SetSwitch("Microsoft.EntityFrameworkCore.SqlServer.EnableLegacyTimestampBehavior", true);

        context.Services.Configure<AuthServerOptions>(
            configuration.GetSection("AuthServer")
        );

        context.ConfigureDataProtection(
            hostingEnvironment,
            configuration,
            ResourceryPlatformWorkflowNames.AuthServer
        );

        var disableTransportSecurityRequirement = hostingEnvironment.IsDevelopment()
            || string.Equals(
                Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"),
                "true",
                StringComparison.OrdinalIgnoreCase
            )
            || configuration["ASPNETCORE_URLS"]?.Contains("http://", StringComparison.OrdinalIgnoreCase) == true;

        PreConfigure<OpenIddictBuilder>(builder =>
        {
            builder.AddValidation(options =>
            {
                options.AddAudiences("ResourceryPlatformWorkflow");
                options.UseLocalServer();
                options.UseAspNetCore();
            });

            if (disableTransportSecurityRequirement)
            {
                builder.AddServer(options =>
                {
                    options.UseAspNetCore().DisableTransportSecurityRequirement();
                });
            }
        });

        PreConfigure<OpenIddictServerBuilder>(builder =>
        {
            // Derive issuer from configuration and normalize by removing any trailing slash.
            // This prevents discovery from exposing a trailing-slash issuer when clients expect
            // the issuer without a trailing slash.
            var configuration = context.Services.GetConfiguration();
            var configuredSelfUrl = configuration["App:SelfUrl"] ?? configuration["App__SelfUrl"];

            // Normalize the issuer by trimming trailing slashes from the string directly
            var issuerString = string.IsNullOrWhiteSpace(configuredSelfUrl)
                ? "https://auth.smartserve.ecowas.int"
                : configuredSelfUrl.TrimEnd('/');

            try
            {
                // Create a Uri and then use its components to rebuild it without trailing slash
                var uri = new Uri(issuerString);
                var scheme = uri.Scheme;
                var host = uri.Host;
                var port = uri.Port;
                var path = uri.AbsolutePath.TrimEnd('/');

                // Reconstruct the issuer URI without relying on Uri.AbsoluteUri (which adds trailing slash)
                var normalizedIssuer = port == 80 || port == 443
                    ? $"{scheme}://{host}{path}"
                    : $"{scheme}://{host}:{port}{path}";

                builder.SetIssuer(new Uri(normalizedIssuer));
            }
            catch
            {
                // Fallback to a hard-coded issuer without trailing slash
                builder.SetIssuer(new Uri("https://auth.smartserve.ecowas.int"));
            }
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        Configure<AuthServerOptions>(configuration.GetSection("AuthServer"));

        context.Services.PostConfigure<OpenIddictServerOptions>(options =>
        {
            var config = context.Services.GetConfiguration();
            var authOptions = config.GetSection("AuthServer").Get<AuthServerOptions>();

            options.Issuer = new Uri(authOptions.Authority);
        });
        
        Configure<OpenIddictServerOptions>(options =>
        {
            var accessTokenLifetimeInMinutes = configuration.GetValue<int?>(
                "OpenIddict:AccessTokenLifetimeInMinutes"
            );
            var refreshTokenLifetimeInDays = configuration.GetValue<int?>(
                "OpenIddict:RefreshTokenLifetimeInDays"
            );

            options.AccessTokenLifetime = TimeSpan.FromMinutes(
                accessTokenLifetimeInMinutes.GetValueOrDefault(60)
            );
            options.RefreshTokenLifetime = TimeSpan.FromDays(
                refreshTokenLifetimeInDays.GetValueOrDefault(30)
            );
        });

        ConfigureMicrosoftExternalLogin(context, configuration);

        context.Services.Configure<AuthUiOptions>(configuration.GetSection("AuthUi"));

        // Ensure Razor Pages are enabled
        context.Services.AddRazorPages();

        Configure<AbpBundlingOptions>(options =>
        {
            options.StyleBundles.Configure(
                LeptonXLiteThemeBundles.Styles.Global,
                bundle =>
                {
                    bundle.AddFiles("/global-styles.css");
                }
            );
        });

        Configure<AbpAuditingOptions>(options =>
        {
            //options.IsEnabledForGetRequests = true;
            options.ApplicationName = "AuthServer";
        });

        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
            options.RedirectAllowedUrls.AddRange(
                configuration["App:RedirectAllowedUrls"].Split(',')
            );

            options.Applications["Angular"].RootUrl = configuration["App:ClientUrl"];
            options.Applications["Angular"].Urls[AccountUrlNames.PasswordReset] =
                "account/reset-password";
        });

        Configure<AbpBackgroundJobOptions>(options =>
        {
            options.IsJobExecutionEnabled = false;
        });

        Configure<AbpDistributedCacheOptions>(options =>
        {
            options.KeyPrefix = "ResourceryPlatformWorkflow:";
        });

    }

    private static void ConfigureMicrosoftExternalLogin(
        ServiceConfigurationContext context,
        IConfiguration configuration
    )
    {
        var microsoftSection = configuration.GetSection("Authentication:Microsoft");
        var clientId = GetFirstNonEmptyValue(
            configuration,
            "OAuth:ClientId",
            "Authentication:Microsoft:ClientId",
            "OAuth__ClientId",
            "OAUTH_CLIENT_ID"
        );
        var clientSecret = GetFirstNonEmptyValue(
            configuration,
            "OAuth:ClientSecret",
            "Authentication:Microsoft:ClientSecret",
            "OAuth__ClientSecret",
            "OAUTH_CLIENT_SECRET"
        );

        if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
        {
            return;
        }

        var tenantId = GetFirstNonEmptyValue(
            configuration,
            "TenantId",
            "OAuth:TenantId",
            "Authentication:Microsoft:TenantId",
            "OAuth__TenantId",
            "OAUTH_TENANT_ID"
        ) ?? "organizations";
        var callbackPath = microsoftSection["CallbackPath"] ?? "/signin-oidc-microsoft";
        var allowedEmailDomain =
            (microsoftSection["AllowedEmailDomain"] ?? "ecowas.int").Trim().ToLowerInvariant();

        context
            .Services.AddAuthentication()
            .AddOpenIdConnect("Microsoft", options =>
            {
                options.SignInScheme = IdentityConstants.ExternalScheme;
                options.Authority = $"https://login.microsoftonline.com/{tenantId}/v2.0";
                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.CallbackPath = callbackPath;
                options.UsePkce = true;
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;

                options.ClaimActions.MapUniqueJsonKey(ClaimTypes.GivenName, "given_name");
                options.ClaimActions.MapUniqueJsonKey(ClaimTypes.Surname, "family_name");
                options.ClaimActions.MapUniqueJsonKey(ClaimTypes.Name, "name");
                options.ClaimActions.MapUniqueJsonKey("picture", "picture");

                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");
                options.Scope.Add("offline_access");
                options.Scope.Add("phone");
                options.Scope.Add("roles");


                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    ValidateIssuer =
                        !tenantId.Equals("common", StringComparison.OrdinalIgnoreCase)
                        && !tenantId.Equals("organizations", StringComparison.OrdinalIgnoreCase),
                };

                options.Events = new OpenIdConnectEvents
                {
                    OnUserInformationReceived = userInfoContext =>
                    {
                        var identity = userInfoContext.Principal?.Identity as ClaimsIdentity;
                        if (identity == null) return Task.CompletedTask;

                        // Read given_name / family_name directly from the UserInfo JSON payload.
                        // These arrive here AFTER GetClaimsFromUserInfoEndpoint fetches them,
                        // so this fires at the right time (unlike OnTokenValidated which is too early).
                        string givenName = null;
                        string surname = null;

                        if (userInfoContext.User.RootElement.TryGetProperty("given_name", out var givenNameEl)
                            && givenNameEl.ValueKind == System.Text.Json.JsonValueKind.String)
                        {
                            givenName = givenNameEl.GetString();
                        }

                        if (userInfoContext.User.RootElement.TryGetProperty("family_name", out var familyNameEl)
                            && familyNameEl.ValueKind == System.Text.Json.JsonValueKind.String)
                        {
                            surname = familyNameEl.GetString();
                        }

                        // Fallback: split the "name" claim (display name) if individual parts are missing.
                        if (string.IsNullOrWhiteSpace(givenName) || string.IsNullOrWhiteSpace(surname))
                        {
                            var fullName = identity.FindFirst(ClaimTypes.Name)?.Value
                                ?? (userInfoContext.User.RootElement.TryGetProperty("name", out var nameEl)
                                    && nameEl.ValueKind == System.Text.Json.JsonValueKind.String
                                    ? nameEl.GetString()
                                    : null);

                            if (!string.IsNullOrWhiteSpace(fullName))
                            {
                                var parts = fullName.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                                if (string.IsNullOrWhiteSpace(givenName) && parts.Length >= 1)
                                    givenName = parts[0];
                                if (string.IsNullOrWhiteSpace(surname) && parts.Length >= 2)
                                    surname = parts[1];
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(givenName)
                            && !identity.HasClaim(c => c.Type == ClaimTypes.GivenName))
                        {
                            identity.AddClaim(new Claim(ClaimTypes.GivenName, givenName));
                        }

                        if (!string.IsNullOrWhiteSpace(givenName)
                            && !identity.HasClaim(c => c.Type == AbpClaimTypes.Name))
                        {
                            identity.AddClaim(new Claim(AbpClaimTypes.Name, givenName));
                        }

                        if (!string.IsNullOrWhiteSpace(surname)
                            && !identity.HasClaim(c => c.Type == ClaimTypes.Surname))
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Surname, surname));
                        }

                        if (!string.IsNullOrWhiteSpace(surname)
                            && !identity.HasClaim(c => c.Type == AbpClaimTypes.SurName))
                        {
                            identity.AddClaim(new Claim(AbpClaimTypes.SurName, surname));
                        }

                        return Task.CompletedTask;
                    },

                    OnTokenValidated = tokenValidatedContext =>
                    {
                        var principal = tokenValidatedContext.Principal;
                        var identity = principal?.Identity as ClaimsIdentity;

                        var email = principal?.FindFirstValue(ClaimTypes.Email)
                            ?? principal?.FindFirstValue("preferred_username")
                            ?? principal?.FindFirstValue("upn");

                        if (string.IsNullOrWhiteSpace(email))
                        {
                            tokenValidatedContext.Fail("Microsoft account did not provide a valid email.");
                            return Task.CompletedTask;
                        }

                        var domain = email.Split('@').LastOrDefault()?.Trim().ToLowerInvariant();
                        if (!string.Equals(domain, allowedEmailDomain, StringComparison.OrdinalIgnoreCase))
                        {
                            tokenValidatedContext.Fail(
                                $"Only {allowedEmailDomain} accounts are allowed to sign in."
                            );
                            return Task.CompletedTask;
                        }

                        if (identity != null && !identity.HasClaim(c => c.Type == ClaimTypes.Email))
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Email, email));
                        }

                        var givenName = principal?.FindFirstValue(ClaimTypes.GivenName)
                            ?? principal?.FindFirstValue("given_name");
                        var surname = principal?.FindFirstValue(ClaimTypes.Surname)
                            ?? principal?.FindFirstValue("family_name");

                        if (string.IsNullOrWhiteSpace(givenName) || string.IsNullOrWhiteSpace(surname))
                        {
                            var fullName = principal?.FindFirstValue(ClaimTypes.Name)
                                ?? principal?.FindFirstValue("name");
                            if (!string.IsNullOrWhiteSpace(fullName))
                            {
                                var parts = fullName.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                                if (string.IsNullOrWhiteSpace(givenName) && parts.Length >= 1)
                                {
                                    givenName = parts[0];
                                }

                                if (string.IsNullOrWhiteSpace(surname) && parts.Length >= 2)
                                {
                                    surname = parts[1];
                                }
                            }
                        }

                        if (
                            identity != null
                            && !string.IsNullOrWhiteSpace(givenName)
                            && !identity.HasClaim(c => c.Type == ClaimTypes.GivenName)
                        )
                        {
                            identity.AddClaim(new Claim(ClaimTypes.GivenName, givenName));
                        }

                        if (
                            identity != null
                            && !string.IsNullOrWhiteSpace(givenName)
                            && !identity.HasClaim(c => c.Type == AbpClaimTypes.Name)
                        )
                        {
                            identity.AddClaim(new Claim(AbpClaimTypes.Name, givenName));
                        }

                        if (
                            identity != null
                            && !string.IsNullOrWhiteSpace(surname)
                            && !identity.HasClaim(c => c.Type == ClaimTypes.Surname)
                        )
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Surname, surname));
                        }

                        if (
                            identity != null
                            && !string.IsNullOrWhiteSpace(surname)
                            && !identity.HasClaim(c => c.Type == AbpClaimTypes.SurName)
                        )
                        {
                            identity.AddClaim(new Claim(AbpClaimTypes.SurName, surname));
                        }

                        var picture = principal?.FindFirstValue("picture")
                            ?? principal?.FindFirstValue("photo");
                        if (
                            identity != null
                            && !string.IsNullOrWhiteSpace(picture)
                            && !identity.HasClaim(c => c.Type == "picture")
                        )
                        {
                            identity.AddClaim(new Claim("picture", picture));
                        }

                        return Task.CompletedTask;
                    },
                };
            });
    }

    private static string GetFirstNonEmptyValue(IConfiguration configuration, params string[] keys)
    {
        foreach (var key in keys)
        {
            var value = configuration[key];
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            var envValue = Environment.GetEnvironmentVariable(key);
            if (!string.IsNullOrWhiteSpace(envValue))
            {
                return envValue;
            }

            if (DotEnvValues.Value.TryGetValue(key, out var dotEnvValue)
                && !string.IsNullOrWhiteSpace(dotEnvValue))
            {
                return dotEnvValue;
            }
        }

        return null;
    }

    private static Dictionary<string, string> LoadDotEnvValues()
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory != null)
        {
            var dotEnvPath = Path.Combine(directory.FullName, ".env");
            if (File.Exists(dotEnvPath))
            {
                foreach (var rawLine in File.ReadAllLines(dotEnvPath))
                {
                    var line = rawLine.Trim();
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    {
                        continue;
                    }

                    var separatorIndex = line.IndexOf('=');
                    if (separatorIndex <= 0)
                    {
                        continue;
                    }

                    var key = line[..separatorIndex].Trim();
                    var value = line[(separatorIndex + 1)..].Trim();

                    if (value.Length >= 2
                        && ((value.StartsWith('"') && value.EndsWith('"'))
                            || (value.StartsWith('\'') && value.EndsWith('\''))))
                    {
                        value = value[1..^1];
                    }

                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        values[key] = value;
                    }
                }

                break;
            }

            directory = directory.Parent;
        }

        return values;
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        IdentityModelEventSource.ShowPII = true;
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();

        if (!env.IsDevelopment())
        {
            app.UseErrorPage();
        }

        app.UseCorrelationId();
        app.UseForwardedHeaders();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseMiddleware<DiscoveryDocumentNormalizationMiddleware>();
        app.UseMiddleware<PostLogoutRedirectUriNormalizationMiddleware>();
        // Health endpoint is mapped below via UseEndpoints to ensure correct IEndpointRouteBuilder is used.
        app.UseCors();
        app.UseAuthentication();
        app.UseMiddleware<ExternalProfileSynchronizationMiddleware>();
        app.UseAbpOpenIddictValidation();
        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseUnitOfWork();
        app.UseAuthorization();
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();

        // Ensure health checks and Razor Pages are mapped
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("/health");
            endpoints.MapRazorPages();
        });
    }
}
