using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace ResourceryPlatformWorkflow.Workflow.HttpApi.Client.ConsoleTestApp;

[DependsOn(typeof(AbpAutofacModule))]
[DependsOn(typeof(WorkflowHttpApiClientModule))]
[DependsOn(typeof(AbpHttpClientIdentityModelModule))]
public class WorkflowConsoleApiClientModule : AbpModule { }
