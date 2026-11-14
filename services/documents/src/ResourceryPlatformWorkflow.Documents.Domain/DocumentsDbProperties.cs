namespace ResourceryPlatformWorkflow.Documents;

public static class DocumentsDbProperties
{
    public const string ConnectionStringName = "ResourceryPlatformWorkflowDocumentsDb";
    public static string DbTablePrefix { get; set; } = "Documents";

    public static string DbSchema { get; set; } = null;
}
