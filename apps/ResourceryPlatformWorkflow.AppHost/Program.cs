using Microsoft.Extensions.Hosting;
using Projects;

namespace ResourceryPlatformWorkflow.AppHost;

internal class Program
{
    private static void Main(string[] args)
    {
        const string LaunchProfileName = "Aspire";
        var builder = DistributedApplication.CreateBuilder(args);

        var postgres = builder.AddPostgres(ResourceryPlatformWorkflowNames.Postgres).WithPgWeb();
        var rabbitMq = builder.AddRabbitMQ(ResourceryPlatformWorkflowNames.RabbitMq).WithManagementPlugin();
        var redis = builder.AddRedis(ResourceryPlatformWorkflowNames.Redis).WithRedisCommander();
        var seq = builder.AddSeq(ResourceryPlatformWorkflowNames.Seq);

        var adminDb = postgres.AddDatabase(ResourceryPlatformWorkflowNames.AdministrationDb);
        var identityDb = postgres.AddDatabase(ResourceryPlatformWorkflowNames.IdentityServiceDb);
        var workflowDb = postgres.AddDatabase(ResourceryPlatformWorkflowNames.WorkflowDb);
        var saasDb = postgres.AddDatabase(ResourceryPlatformWorkflowNames.SaaSDb);

        var migrator = builder
            .AddProject<ResourceryPlatformWorkflow_DbMigrator>(
                ResourceryPlatformWorkflowNames.DbMigrator,
                launchProfileName: LaunchProfileName
            )
            .WithReference(adminDb)
            .WithReference(identityDb)
            .WithReference(workflowDb)
            .WithReference(saasDb)
            .WithReference(seq)
            .WaitFor(postgres);

        var admin = builder
            .AddProject<ResourceryPlatformWorkflow_Administration_HttpApi_Host>(
                ResourceryPlatformWorkflowNames.AdministrationApi,
                launchProfileName: LaunchProfileName
            )
            .WithExternalHttpEndpoints()
            .WithReference(adminDb)
            .WithReference(identityDb)
            .WithReference(rabbitMq)
            .WithReference(redis)
            .WithReference(seq)
            .WaitForCompletion(migrator);

        var identity = builder
            .AddProject<ResourceryPlatformWorkflow_IdentityService_HttpApi_Host>(
                ResourceryPlatformWorkflowNames.IdentityServiceApi,
                launchProfileName: LaunchProfileName
            )
            .WithExternalHttpEndpoints()
            .WithReference(adminDb)
            .WithReference(identityDb)
            .WithReference(saasDb)
            .WithReference(rabbitMq)
            .WithReference(redis)
            .WithReference(seq)
            .WaitForCompletion(migrator);

        var saas = builder
            .AddProject<ResourceryPlatformWorkflow_SaaS_HttpApi_Host>(
                ResourceryPlatformWorkflowNames.SaaSApi,
                launchProfileName: LaunchProfileName
            )
            .WithExternalHttpEndpoints()
            .WithReference(adminDb)
            .WithReference(saasDb)
            .WithReference(rabbitMq)
            .WithReference(redis)
            .WithReference(seq)
            .WaitForCompletion(migrator);

        builder
            .AddProject<ResourceryPlatformWorkflow_Workflow_HttpApi_Host>(
                ResourceryPlatformWorkflowNames.WorkflowApi,
                launchProfileName: LaunchProfileName
            )
            .WithExternalHttpEndpoints()
            .WithReference(adminDb)
            .WithReference(workflowDb)
            .WithReference(rabbitMq)
            .WithReference(redis)
            .WithReference(seq)
            .WaitForCompletion(migrator);

        var gateway = builder
            .AddProject<ResourceryPlatformWorkflow_Gateway>(ResourceryPlatformWorkflowNames.Gateway, launchProfileName: LaunchProfileName)
            .WithExternalHttpEndpoints()
            .WithReference(seq)
            .WaitFor(admin)
            .WaitFor(identity)
            .WaitFor(saas);

        var authserver = builder
            .AddProject<ResourceryPlatformWorkflow_AuthServer>(
                ResourceryPlatformWorkflowNames.AuthServer,
                launchProfileName: LaunchProfileName
            )
            .WithExternalHttpEndpoints()
            .WithReference(adminDb)
            .WithReference(identityDb)
            .WithReference(saasDb)
            .WithReference(rabbitMq)
            .WithReference(redis)
            .WithReference(seq)
            .WaitForCompletion(migrator);

        builder
            .AddProject<ResourceryPlatformWorkflow_WebApp_Blazor>(
                ResourceryPlatformWorkflowNames.WebAppClient,
                launchProfileName: LaunchProfileName
            )
            .WithExternalHttpEndpoints()
            .WithReference(seq)
            .WaitFor(authserver)
            .WaitFor(gateway);

        builder.Build().Run();
    }
}
