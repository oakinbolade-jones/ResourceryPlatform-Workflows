using System;
using System.Text.Json.Serialization;
using Volo.Abp.Application.Dtos;

namespace ResourceryPlatformWorkflow.Workflow.Transcriptions;

public class TranscriptionDto : FullAuditedEntityDto<Guid>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public bool PublishedToWebCast { get; set; }
    public DateTime DateOfTranscription { get; set; }
    public DateTime? EventDate { get; set; }
    public string MediaFile { get; set; }
    public string Language { get; set; } = string.Empty;
    public string InputeFormat { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public InputSource InputSource { get; set; }
    public string SourceReferenceId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public byte[] DocumentData { get; set; }
        public string DocumentExtension { get; set; } = string.Empty;
    public string Transcript { get; set; } = string.Empty;
    public string LinkJson { get; set; } = string.Empty;
    public string LinkSrt { get; set; } = string.Empty;
    public string LinkHtml { get; set; } = string.Empty;
    public string LinkToVideo { get; set; } = string.Empty;
    public string LinkTxt { get; set; } = string.Empty;
    public string LinkDocx { get; set; } = string.Empty;
    public string LinkVerbatimDocx { get; set; } = string.Empty;
}
