using ResourceryPlatformWorkflow.Administration;
using ResourceryPlatformWorkflow.Administration.EntityFrameworkCore;
using ResourceryPlatformWorkflow.IdentityService;
using ResourceryPlatformWorkflow.IdentityService.EntityFrameworkCore;
 using ResourceryPlatformWorkflow.Workflow;
using ResourceryPlatformWorkflow.Workflow.EntityFrameworkCore;
using ResourceryPlatformWorkflow.SaaS;
using ResourceryPlatformWorkflow.SaaS.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict.Tokens;

namespace ResourceryPlatformWorkflow.DbMigrator;

[DependsOn(typeof(AbpAutofacModule))]
[DependsOn(typeof(AbpBackgroundJobsAbstractionsModule))]
[DependsOn(typeof(AdministrationEntityFrameworkCoreModule))]
[DependsOn(typeof(AdministrationApplicationContractsModule))]
[DependsOn(typeof(IdentityServiceEntityFrameworkCoreModule))]
[DependsOn(typeof(IdentityServiceApplicationContractsModule))]
[DependsOn(typeof(WorkflowEntityFrameworkCoreModule))]
[DependsOn(typeof(WorkflowApplicationContractsModule))]
[DependsOn(typeof(SaaSEntityFrameworkCoreModule))]
[DependsOn(typeof(SaaSApplicationContractsModule))]
//[DependsOn(typeof(WebAppEntityFrameworkCoreModule))]
//[DependsOn(typeof(WebAppApplicationContractsModule))]
public class ResourceryPlatformWorkflowDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
        Configure<TokenCleanupOptions>(options => options.IsCleanupEnabled = false);
    }
}
