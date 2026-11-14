namespace ResourceryPlatformWorkflow.Mailing;

public static class MailingDbProperties
{
    public const string ConnectionStringName = "ResourceryPlatformWorkflowMailingDb";
    public static string DbTablePrefix { get; set; } = "Mailing";

    public static string DbSchema { get; set; } = null;
}
