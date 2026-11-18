namespace ResourceryPlatformWorkflow.Workflow.Translations;

public class SummarizationResultDto
{
    public string OriginalText { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public string? Language { get; set; }

    public int SentenceCount { get; set; }
}
