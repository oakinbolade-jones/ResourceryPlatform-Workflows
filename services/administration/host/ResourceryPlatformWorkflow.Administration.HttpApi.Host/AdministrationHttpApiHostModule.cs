using System.IO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using ResourceryPlatformWorkflow;
using ResourceryPlatformWorkflow.Administration.EntityFrameworkCore;
using ResourceryPlatformWorkflow.Auth;
using ResourceryPlatformWorkflow.IdentityService;
using ResourceryPlatformWorkflow.IdentityService.EntityFrameworkCore;
using ResourceryPlatformWorkflow.MultiTenancy;
using ResourceryPlatformWorkflow.SaaS;
using ResourceryPlatformWorkflow.Workflow;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.UI.MultiTenancy;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace ResourceryPlatformWorkflow.Administration;

[DependsOn(typeof(AbpAspNetCoreMvcUiMultiTenancyModule))]
[DependsOn(typeof(AbpIdentityDomainModule))]
[DependsOn(typeof(AdministrationApplicationModule))]
[DependsOn(typeof(AdministrationEntityFrameworkCoreModule))]
[DependsOn(typeof(AdministrationHttpApiModule))]
[DependsOn(typeof(IdentityServiceApplicationContractsModule))]
[DependsOn(typeof(IdentityServiceEntityFrameworkCoreModule))]
[DependsOn(typeof(SaaSApplicationContractsModule))]
[DependsOn(typeof(WorkflowApplicationContractsModule))]
[DependsOn(typeof(ResourceryPlatformWorkflowMicroserviceModule))]
[DependsOn(typeof(ResourceryPlatformWorkflowServiceDefaultsModule))]
public class AdministrationHttpApiHostModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        context.Services.Configure<AuthServerOptions>(
            configuration.GetSection("AuthServer")
        );

        var authOptions = configuration.GetSection("AuthServer").Get<AuthServerOptions>();
        context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);

        context.ConfigureMicroservice(ResourceryPlatformWorkflowNames.AdministrationApi);

        var enableVirtualFileSystem = configuration.GetValue<bool>("ENABLE_VIRTUAL_FILE_SYSTEM", false);

        if (hostingEnvironment.IsDevelopment() && enableVirtualFileSystem)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<AdministrationDomainSharedModule>(
                    Path.Combine(
                        hostingEnvironment.ContentRootPath,
                        string.Format(
                            "..{0}..{0}src{0}ResourceryPlatformWorkflow.Administration.Domain.Shared",
                            Path.DirectorySeparatorChar
                        )
                    )
                );
                options.FileSets.ReplaceEmbeddedByPhysical<AdministrationDomainModule>(
                    Path.Combine(
                        hostingEnvironment.ContentRootPath,
                        string.Format(
                            "..{0}..{0}src{0}ResourceryPlatformWorkflow.Administration.Domain",
                            Path.DirectorySeparatorChar
                        )
                    )
                );
                options.FileSets.ReplaceEmbeddedByPhysical<AdministrationApplicationContractsModule>(
                    Path.Combine(
                        hostingEnvironment.ContentRootPath,
                        string.Format(
                            "..{0}..{0}src{0}ResourceryPlatformWorkflow.Administration.Application.Contracts",
                            Path.DirectorySeparatorChar
                        )
                    )
                );
                options.FileSets.ReplaceEmbeddedByPhysical<AdministrationApplicationModule>(
                    Path.Combine(
                        hostingEnvironment.ContentRootPath,
                        string.Format(
                            "..{0}..{0}src{0}ResourceryPlatformWorkflow.Administration.Application",
                            Path.DirectorySeparatorChar
                        )
                    )
                );
            });
        }
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

        app.UseHttpsRedirection();
        app.UseCorrelationId();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseAbpRequestLocalization();
        app.UseAuthorization();
        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Administration API");
            var configuration = context.GetConfiguration();
            options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
            options.OAuthClientSecret(configuration["AuthServer:SwaggerClientSecret"]);
            options.OAuthScopes("ResourceryPlatformWorkflowAdministration");
        });
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();
    }
}
