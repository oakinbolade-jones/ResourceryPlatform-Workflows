using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

public class ServiceWorkflowHistory : Entity<Guid>
{
    public Guid ServiceWorkflowInstanceId { get; private set; }
    public Guid? ServiceWorkflowStepId { get; private set; }
    public Guid? ServiceWorkflowTaskId { get; private set; }
    public ServiceWorkflowHistoryType Type { get; private set; }
    public string Action { get; private set; }
    public string Comment { get; private set; }
    public Guid? PerformedByUserId { get; private set; }
    public DateTime PerformedAt { get; private set; }

    protected ServiceWorkflowHistory() { }

    public ServiceWorkflowHistory(
        Guid id,
        Guid serviceWorkflowInstanceId,
        ServiceWorkflowHistoryType type,
        string action,
        string comment,
        Guid? performedByUserId,
        Guid? serviceWorkflowStepId = null,
        Guid? serviceWorkflowTaskId = null
    )
        : base(id)
    {
        Check.NotNull(serviceWorkflowInstanceId, nameof(serviceWorkflowInstanceId));

        ServiceWorkflowInstanceId = serviceWorkflowInstanceId;
        ServiceWorkflowStepId = serviceWorkflowStepId;
        ServiceWorkflowTaskId = serviceWorkflowTaskId;
        Type = type;
        SetAction(action);
        SetComment(comment);
        PerformedByUserId = performedByUserId;
        PerformedAt = DateTime.UtcNow;
    }

    public void SetAction(string action)
    {
        Action = Check.NotNullOrWhiteSpace(
            action,
            nameof(action),
            ServiceWorkflowConsts.MaxHistoryActionLength
        );
    }

    public void SetComment(string comment)
    {
        Comment = Check.NotNullOrWhiteSpace(
            comment,
            nameof(comment),
            ServiceWorkflowConsts.MaxHistoryCommentLength
        );
    }
}
