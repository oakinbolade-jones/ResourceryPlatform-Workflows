using System.ComponentModel.DataAnnotations;

namespace ResourceryPlatformWorkflow.Workflow.Translations;

public class SummarizeTextInputDto
{
    [Required]
    public string Text { get; set; } = string.Empty;

    public string? Language { get; set; }

    [Range(1, 20)]
    public int SentenceCount { get; set; } = 5;
}
