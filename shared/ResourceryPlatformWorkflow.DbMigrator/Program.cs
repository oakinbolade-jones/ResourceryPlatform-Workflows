using Serilog;
using ResourceryPlatformWorkflow.Administration.EntityFrameworkCore;
using ResourceryPlatformWorkflow.Workflow.EntityFrameworkCore;
using ResourceryPlatformWorkflow.SaaS.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;

namespace ResourceryPlatformWorkflow.DbMigrator;

internal class Program
{
    private static async Task Main(string[] args)
    {
        ResourceryPlatformWorkflowLogging.Initialize();

        var builder = Host.CreateApplicationBuilder(args);

        builder.AddServiceDefaults();

        builder.AddSqlServerDbContext<AdministrationDbContext>(
            connectionName: ResourceryPlatformWorkflowNames.AdministrationDb
        );
        builder.AddSqlServerDbContext<IdentityDbContext>(connectionName: ResourceryPlatformWorkflowNames.IdentityServiceDb);
        builder.AddSqlServerDbContext<SaaSDbContext>(connectionName: ResourceryPlatformWorkflowNames.SaaSDb);
        builder.AddSqlServerDbContext<WorkflowDbContext>(connectionName: ResourceryPlatformWorkflowNames.WorkflowDb);

        builder.Configuration.AddAppSettingsSecretsJson();

        builder.Logging.AddSerilog();

        builder.Services.AddHostedService<DbMigratorHostedService>();

        var host = builder.Build();

        await host.RunAsync();
    }
}
