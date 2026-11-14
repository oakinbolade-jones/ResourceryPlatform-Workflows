using ResourceryPlatformWorkflow.Mailing.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace ResourceryPlatformWorkflow.Mailing;

[DependsOn(typeof(AbpValidationModule))]
public class MailingDomainSharedModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<MailingDomainSharedModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options
                .Resources.Add<MailingResource>("en")
                .AddBaseTypes(typeof(AbpValidationResource))
                .AddVirtualJson("/Localization/Mailing");
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("Mailing", typeof(MailingResource));
        });
    }
}
