namespace ResourceryPlatformWorkflow.Workflow.Translations;

public class AzureLanguageServiceOptions
{
    public const string SectionName = "Azure:LanguageServices";

    public string Endpoint { get; set; } = string.Empty;

    public string Key { get; set; } = string.Empty;

    public string Key1 { get; set; } = string.Empty;

    public string Key2 { get; set; } = string.Empty;

    public int DefaultSentenceCount { get; set; } = 5;

    public int PollingTimeoutSeconds { get; set; } = 60;
}
