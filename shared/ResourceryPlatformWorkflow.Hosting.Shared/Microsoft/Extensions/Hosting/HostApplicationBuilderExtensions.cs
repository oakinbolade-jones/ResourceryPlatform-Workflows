using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.Hosting;

public static class HostApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddSharedEndpoints(this IHostApplicationBuilder builder)
    {
        builder.AddRabbitMQClient(
            connectionName: ResourceryPlatformWorkflowNames.RabbitMq,
            action =>
                action.ConnectionString = builder.Configuration.GetConnectionString(
                    ResourceryPlatformWorkflowNames.RabbitMq
                )
        );
        builder.AddRedisDistributedCache(connectionName: ResourceryPlatformWorkflowNames.Redis);
        builder.AddSeqEndpoint(connectionName: ResourceryPlatformWorkflowNames.Seq);

        return builder;
    }
}
