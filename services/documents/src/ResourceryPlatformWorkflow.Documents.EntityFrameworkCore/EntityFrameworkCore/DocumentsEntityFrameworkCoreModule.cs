using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.PostgreSql;
using Volo.Abp.Modularity;

namespace ResourceryPlatformWorkflow.Documents.EntityFrameworkCore;

[DependsOn(typeof(DocumentsDomainModule))]
[DependsOn(typeof(AbpEntityFrameworkCorePostgreSqlModule))]
[DependsOn(typeof(ResourceryPlatformWorkflowSharedModule))]
public class DocumentsEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // https://www.npgsql.org/efcore/release-notes/6.0.html#opting-out-of-the-new-timestamp-mapping-logic
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        Configure<AbpDbConnectionOptions>(options =>
        {
            options.Databases.Configure(ResourceryPlatformWorkflowNames.DocumentsDb, db => { });
        });

        Configure<AbpDbContextOptions>(options =>
        {
            options.UseNpgsql();
        });

        context.Services.AddAbpDbContext<DocumentsDbContext>(options =>
        {
            options.AddDefaultRepositories(true);
        });
    }
}
