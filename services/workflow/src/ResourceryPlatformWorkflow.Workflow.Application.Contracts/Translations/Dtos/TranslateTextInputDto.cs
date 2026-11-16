using System.ComponentModel.DataAnnotations;

namespace ResourceryPlatformWorkflow.Workflow.Translations;

public class TranslateTextInputDto
{
    [Required]
    public string Text { get; set; } = string.Empty;

    [Required]
    public string TargetLanguage { get; set; } = string.Empty;

    public string? SourceLanguage { get; set; }
}