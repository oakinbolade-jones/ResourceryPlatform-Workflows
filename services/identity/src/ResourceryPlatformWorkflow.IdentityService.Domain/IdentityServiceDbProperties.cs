namespace ResourceryPlatformWorkflow.IdentityService;

public static class IdentityServiceDbProperties
{
    public const string ConnectionStringName = "ResourceryPlatformWorkflowIdentityServiceDb";
    public static string DbTablePrefix { get; set; } = "IdentityService";

    public static string DbSchema { get; set; } = null;
}
