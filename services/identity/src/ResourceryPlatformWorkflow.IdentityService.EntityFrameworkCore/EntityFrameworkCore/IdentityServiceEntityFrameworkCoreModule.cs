using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict.EntityFrameworkCore;

namespace ResourceryPlatformWorkflow.IdentityService.EntityFrameworkCore;

[DependsOn(typeof(AbpEntityFrameworkCoreSqlServerModule))]
[DependsOn(typeof(AbpIdentityEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpOpenIddictEntityFrameworkCoreModule))]
[DependsOn(typeof(IdentityServiceDomainModule))]
[DependsOn(typeof(ResourceryPlatformWorkflowSharedModule))]
public class IdentityServiceEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        AppContext.SetSwitch("Microsoft.EntityFrameworkCore.SqlServer.EnableLegacyTimestampBehavior", true);

        Configure<AbpDbConnectionOptions>(options =>
        {
            options.Databases.Configure(
                ResourceryPlatformWorkflowNames.IdentityServiceDb,
                db =>
                {
                    db.MappedConnections.Add("AbpIdentity");
                    db.MappedConnections.Add("AbpOpenIddict");
                }
            );
        });

        Configure<AbpDbContextOptions>(options =>
        {
            options.UseSqlServer();
        });

        context.Services.AddAbpDbContext<IdentityServiceDbContext>(options =>
        {
            options.ReplaceDbContext<IIdentityDbContext>();
            options.ReplaceDbContext<IOpenIddictDbContext>();

            options.AddDefaultRepositories(true);
        });
    }
}
