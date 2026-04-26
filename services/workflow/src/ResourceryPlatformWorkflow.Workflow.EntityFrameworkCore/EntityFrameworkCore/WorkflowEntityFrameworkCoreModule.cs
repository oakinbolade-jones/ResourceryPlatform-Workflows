using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.Modularity;

namespace ResourceryPlatformWorkflow.Workflow.EntityFrameworkCore;

[DependsOn(typeof(WorkflowDomainModule))]
[DependsOn(typeof(AbpEntityFrameworkCoreSqlServerModule))]
[DependsOn(typeof(ResourceryPlatformWorkflowSharedModule))]
public class WorkflowEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        AppContext.SetSwitch("Microsoft.EntityFrameworkCore.SqlServer.EnableLegacyTimestampBehavior", true);

        Configure<AbpDbConnectionOptions>(options =>
        {
            options.Databases.Configure(ResourceryPlatformWorkflowNames.WorkflowDb, db => { });
        });

        Configure<AbpDbContextOptions>(options =>
        {
            options.UseSqlServer();
        });

        context.Services.AddAbpDbContext<WorkflowDbContext>(options =>
        {
            options.AddDefaultRepositories(true);
        });
    }
}
