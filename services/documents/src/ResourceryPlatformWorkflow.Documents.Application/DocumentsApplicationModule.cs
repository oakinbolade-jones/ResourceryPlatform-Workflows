using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace ResourceryPlatformWorkflow.Documents;

[DependsOn(typeof(DocumentsDomainModule))]
[DependsOn(typeof(DocumentsApplicationContractsModule))]
[DependsOn(typeof(AbpDddApplicationModule))]
[DependsOn(typeof(AbpAutoMapperModule))]
public class DocumentsApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<DocumentsApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<DocumentsApplicationModule>(true);
        });
    }
}
