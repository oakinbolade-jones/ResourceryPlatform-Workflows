using ResourceryPlatformWorkflow.Documents.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace ResourceryPlatformWorkflow.Documents;

[DependsOn(typeof(AbpValidationModule))]
public class DocumentsDomainSharedModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<DocumentsDomainSharedModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options
                .Resources.Add<DocumentsResource>("en")
                .AddBaseTypes(typeof(AbpValidationResource))
                .AddVirtualJson("/Localization/Documents");
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("Documents", typeof(DocumentsResource));
        });
    }
}
