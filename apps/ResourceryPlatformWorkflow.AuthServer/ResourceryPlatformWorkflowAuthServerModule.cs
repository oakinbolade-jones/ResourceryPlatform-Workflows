using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        var clientId = microsoftSection["ClientId"];
        var clientSecret = microsoftSection["ClientSecret"];

        if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
        {
            return;
        }

        var tenantId = microsoftSection["TenantId"] ?? "organizations";
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
        app. mergeUseAbpOpenIddictValidation();

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseUnitOfWork();
        app.UseAuthorization();
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();
    }
}
