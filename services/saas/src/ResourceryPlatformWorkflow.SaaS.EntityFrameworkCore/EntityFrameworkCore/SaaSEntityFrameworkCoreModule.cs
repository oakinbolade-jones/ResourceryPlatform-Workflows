using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace ResourceryPlatformWorkflow.SaaS.EntityFrameworkCore;

[DependsOn(typeof(AbpEntityFrameworkCoreSqlServerModule))]
[DependsOn(typeof(AbpTenantManagementEntityFrameworkCoreModule))]
[DependsOn(typeof(SaaSDomainModule))]
[DependsOn(typeof(ResourceryPlatformWorkflowSharedModule))]
public class SaaSEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        AppContext.SetSwitch("Microsoft.EntityFrameworkCore.SqlServer.EnableLegacyTimestampBehavior", true);

        Configure<AbpDbConnectionOptions>(options =>
        {
            options.Databases.Configure(
                ResourceryPlatformWorkflowNames.SaaSDb,
                db =>
                {
                    db.MappedConnections.Add("AbpTenantManagement");
                }
            );
        });

        Configure<AbpDbContextOptions>(options =>
        {
            options.UseSqlServer();
        });

        context.Services.AddAbpDbContext<SaaSDbContext>(options =>
        {
            options.ReplaceDbContext<ITenantManagementDbContext>();

            options.AddDefaultRepositories(true);
        });
    }
}
