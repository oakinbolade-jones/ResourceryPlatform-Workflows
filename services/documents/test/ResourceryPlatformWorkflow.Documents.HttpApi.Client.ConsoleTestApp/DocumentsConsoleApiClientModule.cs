using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace ResourceryPlatformWorkflow.Documents.HttpApi.Client.ConsoleTestApp;

[DependsOn(typeof(AbpAutofacModule))]
[DependsOn(typeof(DocumentsHttpApiClientModule))]
[DependsOn(typeof(AbpHttpClientIdentityModelModule))]
public class DocumentsConsoleApiClientModule : AbpModule { }
