using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace ResourceryPlatformWorkflow.Documents;

[DependsOn(typeof(DocumentsApplicationContractsModule))]
[DependsOn(typeof(AbpHttpClientModule))]
public class DocumentsHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(DocumentsApplicationContractsModule).Assembly,
            DocumentsRemoteServiceConsts.RemoteServiceName
        );

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<DocumentsHttpApiClientModule>();
        });
    }
}
