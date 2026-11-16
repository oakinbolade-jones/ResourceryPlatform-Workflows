namespace ResourceryPlatformWorkflow.Workflow.Translations;

public class TranslationResultDto
{
    public string OriginalText { get; set; } = string.Empty;

    public string TranslatedText { get; set; } = string.Empty;

    public string? SourceLanguage { get; set; }

    public string TargetLanguage { get; set; } = string.Empty;
}