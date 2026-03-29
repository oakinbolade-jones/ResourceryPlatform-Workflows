using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

public class ServiceWorkflowInstance : FullAuditedAggregateRoot<Guid>
{
    public Guid ServiceWorkflowId { get; private set; }
    public Guid RequestId { get; private set; }
    public Guid? CurrentStepId { get; private set; }
    public ServiceWorkflowInstanceStatus Status { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    public ICollection<ServiceWorkflowTask> Tasks { get; private set; }
    public ICollection<ServiceWorkflowHistory> HistoryEntries { get; private set; }

    protected ServiceWorkflowInstance()
    {
        Tasks = new List<ServiceWorkflowTask>();
        HistoryEntries = new List<ServiceWorkflowHistory>();
    }

    public ServiceWorkflowInstance(Guid id, Guid serviceWorkflowId, Guid requestId)
        : base(id)
    {
        Check.NotNull(serviceWorkflowId, nameof(serviceWorkflowId));
        Check.NotNull(requestId, nameof(requestId));

        ServiceWorkflowId = serviceWorkflowId;
        RequestId = requestId;
        Status = ServiceWorkflowInstanceStatus.Pending;
        StartedAt = DateTime.UtcNow;

        Tasks = new List<ServiceWorkflowTask>();
        HistoryEntries = new List<ServiceWorkflowHistory>();
    }

    public void StartStep(Guid stepId)
    {
        Check.NotNull(stepId, nameof(stepId));

        CurrentStepId = stepId;
        if (Status == ServiceWorkflowInstanceStatus.Pending)
        {
            Status = ServiceWorkflowInstanceStatus.InProgress;
        }
    }

    public void Complete()
    {
        Status = ServiceWorkflowInstanceStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        Status = ServiceWorkflowInstanceStatus.Rejected;
        CompletedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = ServiceWorkflowInstanceStatus.Cancelled;
        CompletedAt = DateTime.UtcNow;
    }

    public void AddTask(ServiceWorkflowTask task)
    {
        Tasks.Add(Check.NotNull(task, nameof(task)));
    }

    public void AddHistory(ServiceWorkflowHistory history)
    {
        HistoryEntries.Add(Check.NotNull(history, nameof(history)));
    }
}
