using System;
using Volo.Abp.BackgroundJobs;

namespace ResourceryPlatformWorkflow.Workflow.Transcriptions;

[BackgroundJobName("workflow.transcription.submit-to-wipo")]
public class SubmitToWipoBackgroundJobArgs
{
    public Guid? TranscriptionId { get; set; }
    public string SourceReferenceId { get; set; }
    public string Language { get; set; }
    public string InputFormat { get; set; }
    public string FilePath { get; set; }
    public string FileName { get; set; }
    public string FileContentType { get; set; }
}
