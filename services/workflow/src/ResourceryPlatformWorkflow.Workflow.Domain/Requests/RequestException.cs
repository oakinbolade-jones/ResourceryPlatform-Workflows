using System;
using Volo.Abp;

namespace ResourceryPlatformWorkflow.Workflow.Requests;

public class RequestException : BusinessException
{
    private RequestException(string errorCode)
        : base(errorCode)
    {
    }

    public static RequestException DuplicateDocument(string title)
    {
        var exception = new RequestException(WorkflowErrorCodes.Requests.DuplicateDocument);
        exception.WithData("title", title);
        return exception;
    }

    public static RequestException NotFound(Guid requestId)
    {
        var exception = new RequestException(WorkflowErrorCodes.Requests.RequestNotFound);
        exception.WithData("requestId", requestId);
        return exception;
    }

    public static RequestException InvalidStatus(RequestStatus requestStatus)
    {
        var exception = new RequestException(WorkflowErrorCodes.Requests.InvalidRequestStatus);
        exception.WithData("requestStatus", requestStatus);
        return exception;
    }
}
