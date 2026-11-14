using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ResourceryPlatformWorkflow.Documents.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.UI.MultiTenancy;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.VirtualFileSystem;

namespace ResourceryPlatformWorkflow.Documents;

[DependsOn(typeof(AbpAspNetCoreMvcUiMultiTenancyModule))]
[DependsOn(typeof(AbpAuditLoggingEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpPermissionManagementEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpSettingManagementEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpTenantManagementEntityFrameworkCoreModule))]
[DependsOn(typeof(DocumentsApplicationModule))]
[DependsOn(typeof(DocumentsEntityFrameworkCoreModule))]
[DependsOn(typeof(DocumentsHttpApiModule))]
[DependsOn(typeof(ResourceryPlatformWorkflowMicroserviceModule))]
[DependsOn(typeof(ResourceryPlatformWorkflowServiceDefaultsModule))]
public class DocumentsHttpApiHostModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        context.ConfigureMicroservice(ResourceryPlatformWorkflowNames.DocumentsApi);

        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<DocumentsDomainSharedModule>(
                    Path.Combine(
                        hostingEnvironment.ContentRootPath,
                        string.Format(
                            "..{0}..{0}src{0}ResourceryPlatformWorkflow.Documents.Domain.Shared",
                            Path.DirectorySeparatorChar
                        )
                    )
                );
                options.FileSets.ReplaceEmbeddedByPhysical<DocumentsDomainModule>(
                    Path.Combine(
                        hostingEnvironment.ContentRootPath,
                        string.Format(
                            "..{0}..{0}src{0}ResourceryPlatformWorkflow.Documents.Domain",
                            Path.DirectorySeparatorChar
                        )
                    )
                );
                options.FileSets.ReplaceEmbeddedByPhysical<DocumentsApplicationContractsModule>(
                    Path.Combine(
                        hostingEnvironment.ContentRootPath,
                        string.Format(
                            "..{0}..{0}src{0}ResourceryPlatformWorkflow.Documents.Application.Contracts",
                            Path.DirectorySeparatorChar
                        )
                    )
                );
                options.FileSets.ReplaceEmbeddedByPhysical<DocumentsApplicationModule>(
                    Path.Combine(
                        hostingEnvironment.ContentRootPath,
                        string.Format(
                            "..{0}..{0}src{0}ResourceryPlatformWorkflow.Documents.Application",
                            Path.DirectorySeparatorChar
                        )
                    )
                );
            });
        }
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
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

        app.UseMultiTenancy();

        app.UseAbpRequestLocalization();
        app.UseAuthorization();
        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Documents API");

            var configuration = context.GetConfiguration();
            options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
            options.OAuthClientSecret(configuration["AuthServer:SwaggerClientSecret"]);
            options.OAuthScopes("ResourceryPlatformWorkflowDocuments");
        });
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();
    }
}
