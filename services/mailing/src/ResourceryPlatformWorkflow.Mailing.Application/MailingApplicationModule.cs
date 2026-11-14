using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace ResourceryPlatformWorkflow.Mailing;

[DependsOn(typeof(MailingDomainModule))]
[DependsOn(typeof(MailingApplicationContractsModule))]
[DependsOn(typeof(AbpDddApplicationModule))]
[DependsOn(typeof(AbpAutoMapperModule))]
public class MailingApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<MailingApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<MailingApplicationModule>(true);
        });
    }
}
