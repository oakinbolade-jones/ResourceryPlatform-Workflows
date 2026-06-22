using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using ResourceryPlatformWorkflow.Administration.EntityFrameworkCore;
using ResourceryPlatformWorkflow.SaaS.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;


namespace ResourceryPlatformWorkflow;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        ResourceryPlatformWorkflowLogging.Initialize();

        try
        {
            Log.Information("Starting ResourceryPlatformWorkflow.AuthServer.");

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddHealthChecks();
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor |
                    ForwardedHeaders.XForwardedProto;

                // IMPORTANT: allow nginx reverse proxy trust
                options.KnownNetworks.Clear();
                options.KnownIPNetworks.Clear();
                options.KnownProxies.Clear();

                // optional hardening (recommended)
                options.ForwardLimit = 1;
            });

            builder.AddServiceDefaults();
            builder.AddSharedEndpoints();

            builder.AddSqlServerDbContext<AdministrationDbContext>(
                connectionName: ResourceryPlatformWorkflowNames.AdministrationDb,
                configure => configure.DisableRetry = true
            );
            builder.AddSqlServerDbContext<IdentityDbContext>(
                connectionName: ResourceryPlatformWorkflowNames.IdentityServiceDb,
                configure => configure.DisableRetry = true
            );
            builder.AddSqlServerDbContext<SaaSDbContext>(
                connectionName: ResourceryPlatformWorkflowNames.SaaSDb,
                configure => configure.DisableRetry = true
            );

            builder.Host.AddAppSettingsSecretsJson().UseAutofac().UseSerilog();

            await builder.AddApplicationAsync<ResourceryPlatformWorkflowAuthServerModule>();

            var app = builder.Build();

            await app.InitializeApplicationAsync();

            await app.RunAsync();

            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "ResourceryPlatformWorkflow.AuthServer terminated unexpectedly!");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
