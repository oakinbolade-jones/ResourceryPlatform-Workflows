
namespace ResourceryPlatformWorkflow.Workflow.Translations;

public class AzureTranslatorOptions
{
    public const string SectionName = "AzureTranslator";

    public string Endpoint { get; set; } = string.Empty;

    public string Key { get; set; } = string.Empty;

    public string Region { get; set; } = string.Empty;

    public int MaxBatchSize { get; set; } = 25;

    public int MaxBatchCharacters { get; set; } = 4500;
}
