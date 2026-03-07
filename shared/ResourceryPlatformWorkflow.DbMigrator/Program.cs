using Serilog;
using ResourceryPlatformWorkflow.Administration.EntityFrameworkCore;
// using ResourceryPlatformWorkflow.Projects.EntityFrameworkCore;
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

        builder.AddNpgsqlDbContext<AdministrationDbContext>(
            connectionName: ResourceryPlatformWorkflowNames.AdministrationDb
        );
        builder.AddNpgsqlDbContext<IdentityDbContext>(connectionName: ResourceryPlatformWorkflowNames.IdentityServiceDb);
        builder.AddNpgsqlDbContext<SaaSDbContext>(connectionName: ResourceryPlatformWorkflowNames.SaaSDb);
        //builder.AddNpgsqlDbContext<ProjectsDbContext>(connectionName: ResourceryPlatformWorkflowNames.ProjectsDb);

        builder.Configuration.AddAppSettingsSecretsJson();

        builder.Logging.AddSerilog();

        builder.Services.AddHostedService<DbMigratorHostedService>();

        var host = builder.Build();

        await host.RunAsync();
    }
}
