using System;
using Volo.Abp;

namespace ResourceryPlatformWorkflow.Workflow.Transcriptions;

public class TranscriptionException : BusinessException
{
    private TranscriptionException(string errorCode)
        : base(errorCode)
    {
    }

    public static TranscriptionException NotFound(Guid transcriptionId)
    {
        var exception = new TranscriptionException(WorkflowErrorCodes.Transcriptions.TranscriptionNotFound);
        exception.WithData("transcriptionId", transcriptionId);
        return exception;
    }
}
