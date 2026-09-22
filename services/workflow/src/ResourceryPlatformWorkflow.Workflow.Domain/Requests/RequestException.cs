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

    public static RequestException RequestDataRequired()
    {
        return new RequestException(WorkflowErrorCodes.Requests.RequestDataRequired);
    }

    public static RequestException MeetingDataRequired()
    {
        return new RequestException(WorkflowErrorCodes.Requests.MeetingDataRequired);
    }

    public static RequestException MeetingItemDataRequired(string? itemName = null)
    {
        var exception = new RequestException(WorkflowErrorCodes.Requests.MeetingItemDataRequired);
        if (!string.IsNullOrWhiteSpace(itemName))
        {
            exception.WithData("itemName", itemName);
        }

        return exception;
    }

    public static RequestException InvalidMeetingRequirement(string itemName)
    {
        var exception = new RequestException(WorkflowErrorCodes.Requests.InvalidMeetingRequirement);
        exception.WithData("itemName", itemName);
        return exception;
    }
}
