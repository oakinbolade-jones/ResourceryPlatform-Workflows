using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace ResourceryPlatformWorkflow.Mailing;

[DependsOn(typeof(MailingApplicationContractsModule))]
[DependsOn(typeof(AbpHttpClientModule))]
public class MailingHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(MailingApplicationContractsModule).Assembly,
            MailingRemoteServiceConsts.RemoteServiceName
        );

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<MailingHttpApiClientModule>();
        });
    }
}
