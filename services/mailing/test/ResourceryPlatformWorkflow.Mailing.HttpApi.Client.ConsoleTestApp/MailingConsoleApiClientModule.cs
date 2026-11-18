using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace ResourceryPlatformWorkflow.Mailing.HttpApi.Client.ConsoleTestApp;

[DependsOn(typeof(AbpAutofacModule))]
[DependsOn(typeof(MailingHttpApiClientModule))]
[DependsOn(typeof(AbpHttpClientIdentityModelModule))]
public class MailingConsoleApiClientModule : AbpModule { }
