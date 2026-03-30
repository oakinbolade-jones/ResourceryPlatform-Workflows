using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

public class ServiceWorkflowTask : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; private set; }
    public Guid ServiceWorkflowInstanceId { get; private set; }
    public Guid ServiceWorkflowStepId { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public Guid? AssigneeUserId { get; private set; }
    public ServiceWorkflowTaskStatus Status { get; private set; }
    public DateTime? DueDate { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    protected ServiceWorkflowTask() { }

    public ServiceWorkflowTask(
        Guid id,
        Guid serviceWorkflowInstanceId,
        Guid serviceWorkflowStepId,
        string title,
        string description,
        Guid? assigneeUserId,
        DateTime? dueDate
    )
        : base(id)
    {
        Check.NotNull(serviceWorkflowInstanceId, nameof(serviceWorkflowInstanceId));
        Check.NotNull(serviceWorkflowStepId, nameof(serviceWorkflowStepId));

        ServiceWorkflowInstanceId = serviceWorkflowInstanceId;
        ServiceWorkflowStepId = serviceWorkflowStepId;
        SetTitle(title);
        SetDescription(description);
        SetAssigneeUserId(assigneeUserId);
        SetDueDate(dueDate);
        Status = ServiceWorkflowTaskStatus.Pending;
    }

    public void SetTitle(string title)
    {
        Title = Check.NotNullOrWhiteSpace(title, nameof(title), ServiceWorkflowConsts.MaxTaskTitleLength);
    }

    public void SetDescription(string description)
    {
        Description = Check.NotNullOrWhiteSpace(
            description,
            nameof(description),
            ServiceWorkflowConsts.MaxTaskDescriptionLength
        );
    }

    public void SetAssigneeUserId(Guid? assigneeUserId)
    {
        AssigneeUserId = assigneeUserId;
    }

    public void SetDueDate(DateTime? dueDate)
    {
        DueDate = dueDate;
    }

    public void Start()
    {
        Status = ServiceWorkflowTaskStatus.InProgress;
    }

    public void Complete()
    {
        Status = ServiceWorkflowTaskStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        Status = ServiceWorkflowTaskStatus.Rejected;
        CompletedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = ServiceWorkflowTaskStatus.Cancelled;
        CompletedAt = DateTime.UtcNow;
    }
}
