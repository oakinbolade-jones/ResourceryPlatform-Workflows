using System;
using System.Collections.Generic;
using System.Linq;
using ResourceryPlatformWorkflow.Workflow.Services;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

public class ServiceWorkflow : FullAuditedAggregateRoot<Guid>
{
    public ServiceRelation ServiceRelation { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool IsActive { get; private set; }
    public ICollection<ServiceWorkflowStep> Steps { get; private set; }

    protected ServiceWorkflow()
    {
        Steps = new List<ServiceWorkflowStep>();
    }

    public ServiceWorkflow(Guid id, string name, string description)
        : this(id, Guid.Empty, name, description) { }

    public ServiceWorkflow(Guid id, Guid serviceId, string name, string description)
        : base(id)
    {
        Steps = new List<ServiceWorkflowStep>();
        SetService(serviceId);
        SetName(name);
        SetDescription(description);
        IsActive = true;
    }

    public void SetService(Guid serviceId)
    {
        ServiceRelation = new ServiceRelation(serviceId, Id);
    }

    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), ServiceWorkflowConsts.MaxWorkflowNameLength);
    }

    public void SetDescription(string description)
    {
        Description = Check.NotNullOrWhiteSpace(
            description,
            nameof(description),
            ServiceWorkflowConsts.MaxWorkflowDescriptionLength
        );
    }

    public void SetIsActive(bool isActive)
    {
        IsActive = isActive;
    }

    public void AddStep(Guid stepId, string name, string description, int order)
    {
        Check.NotNull(stepId, nameof(stepId));

        if (order < 1)
        {
            throw new BusinessException(WorkflowErrorCodes.ServiceWorkflows.InvalidStepOrder);
        }

        if (Steps.Any(x => x.Order == order))
        {
            throw new BusinessException(WorkflowErrorCodes.ServiceWorkflows.DuplicateStepOrder);
        }

        Steps.Add(new ServiceWorkflowStep(stepId, Id, name, description, order));
    }
}
