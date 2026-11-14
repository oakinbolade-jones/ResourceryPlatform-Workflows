using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using ResourceryPlatformWorkflow.Documents.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;

namespace ResourceryPlatformWorkflow.Documents;

[DependsOn(typeof(DocumentsApplicationContractsModule))]
[DependsOn(typeof(AbpAspNetCoreMvcModule))]
public class DocumentsHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(DocumentsHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources.Get<DocumentsResource>().AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
