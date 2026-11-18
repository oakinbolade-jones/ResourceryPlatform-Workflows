using System.ComponentModel.DataAnnotations;

namespace ResourceryPlatformWorkflow.Workflow.Summarization.Dtos;

public class SummarizeTranscriptInputDto
{
    [Required]
    public string Transcript { get; set; } = string.Empty;

    public string SummaryType { get; set; } = "DetailedMinutes";

    public string Language { get; set; } = "en";
}