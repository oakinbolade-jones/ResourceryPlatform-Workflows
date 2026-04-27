using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using ResourceryPlatformWorkflow.Administration.EntityFrameworkCore;
using ResourceryPlatformWorkflow.IdentityService.EntityFrameworkCore;
using ResourceryPlatformWorkflow.Middleware;
using ResourceryPlatformWorkflow.MultiTenancy;
using ResourceryPlatformWorkflow.SaaS.EntityFrameworkCore;

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
using Volo.Abp.Localization;
using ResourceryPlatformWorkflow.Workflow;
namespace ResourceryPlatformWorkflow;

// ...existing code...
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
[DependsOn(typeof(WorkflowDomainSharedModule))]
public class ResourceryPlatformWorkflowAuthServerModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        AppContext.SetSwitch("Microsoft.EntityFrameworkCore.SqlServer.EnableLegacyTimestampBehavior", true);

        PreConfigure<OpenIddictBuilder>(builder =>
        {
            builder.AddValidation(options =>
            {
                options.AddAudiences("ResourceryPlatformWorkflow");
                options.UseLocalServer();
                options.UseAspNetCore();
            });
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

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
        // var clientId = microsoftSection["ClientId"]; 
        // var clientSecret = microsoftSection["ClientSecret"]; 

        var clientId = configuration["OAuth:ClientId"];
        var clientSecret = configuration["OAuth:ClientSecret"];

        if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
        {
            return;
        }

        // var tenantId = microsoftSection["TenantId"] ?? "organizations";

        var tenantId = configuration["TenantId"] ?? "organizations";
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
        app.UseStaticFiles();
        app.UseRouting();
        app.UseMiddleware<PostLogoutRedirectUriNormalizationMiddleware>();
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

        // Ensure Razor Pages are mapped
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
        });
    }
}
