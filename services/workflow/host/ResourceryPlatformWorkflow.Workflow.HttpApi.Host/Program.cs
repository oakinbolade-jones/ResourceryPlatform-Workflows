using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using ResourceryPlatformWorkflow.Administration.EntityFrameworkCore;
using ResourceryPlatformWorkflow.Workflow.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace ResourceryPlatformWorkflow.Workflow;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        ResourceryPlatformWorkflowLogging.Initialize();

        try
        {
            Log.Information("Starting web host.");

            var builder = WebApplication.CreateBuilder(args);
            builder.AddServiceDefaults();
            builder.AddSharedEndpoints();

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Limits.MaxRequestBodySize = null; // Allow large video uploads
            });

            builder.AddSqlServerDbContext<AdministrationDbContext>(
                connectionName: ResourceryPlatformWorkflowNames.AdministrationDb,
                configure => configure.DisableRetry = true
            );
            builder.AddSqlServerDbContext<WorkflowDbContext>(
                connectionName: ResourceryPlatformWorkflowNames.WorkflowDb,
                configure => configure.DisableRetry = true
            );

            builder.Host.AddAppSettingsSecretsJson().UseAutofac().UseSerilog();

            await builder.AddApplicationAsync<WorkflowHttpApiHostModule>();

            var app = builder.Build();

            await app.InitializeApplicationAsync();

            await app.RunAsync();

            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly!");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
