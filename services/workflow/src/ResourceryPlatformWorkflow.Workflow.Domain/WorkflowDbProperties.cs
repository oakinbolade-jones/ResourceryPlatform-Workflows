namespace ResourceryPlatformWorkflow.Workflow;

public static class WorkflowDbProperties
{
    public const string ConnectionStringName = "ResourceryPlatformWorkflowWorkflowDb";
    public static string DbTablePrefix { get; set; } = "";

    public static string DbSchema { get; set; } = null;
}
