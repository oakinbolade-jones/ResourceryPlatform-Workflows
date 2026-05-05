using Serilog;
using Serilog.Events;

namespace Microsoft.Extensions.Hosting
{
    public static class ResourceryPlatformWorkflowLogging
    {
        public static void Initialize()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .WriteTo.Async(c => c.File("Logs/logs.txt"))
                .WriteTo.Async(c => c.Console())
                .Enrich.FromLogContext()
                .CreateLogger();
        }
    }
}
