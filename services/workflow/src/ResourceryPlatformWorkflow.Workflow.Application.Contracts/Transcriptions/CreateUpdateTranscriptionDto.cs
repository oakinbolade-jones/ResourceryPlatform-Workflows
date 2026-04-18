using System;

namespace ResourceryPlatformWorkflow.Workflow.Transcriptions;

public class CreateUpdateTranscriptionDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public DateTime DateOfTranscription { get; set; }
    public DateTime? EventDate { get; set; }
    public string MediaFile { get; set; }
    public string Language { get; set; } = "en";
    public string InputeFormat { get; set; } = "webm";
    public string Status { get; set; } = "Pending";
    public InputSource InputSource { get; set; } = InputSource.Upload;
    public string ThumbNailImage { get; set; }
    public string SourceReferenceId { get; set; }

    public string LinkJson { get; set; } = string.Empty;
    public string LinkSrt { get; set; } = string.Empty;
    public string LinkHtml { get; set; } = string.Empty;
    public string LinkTxt { get; set; } = string.Empty;
    public string LinkDocx { get; set; } = string.Empty;
    public string LinkVerbatimDocx { get; set; } = string.Empty;
}
