using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

public class ServiceWorkflowStep : Entity<Guid>
{
    public Guid ServiceWorkflowId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public int Order { get; private set; }

    protected ServiceWorkflowStep() { }

    public ServiceWorkflowStep(Guid id, Guid serviceWorkflowId, string name, string description, int order)
        : base(id)
    {
        ServiceWorkflowId = serviceWorkflowId;
        SetName(name);
        SetDescription(description);
        SetOrder(order);
    }

    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), ServiceWorkflowConsts.MaxStepNameLength);
    }

    public void SetDescription(string description)
    {
        Description = Check.NotNullOrWhiteSpace(
            description,
            nameof(description),
            ServiceWorkflowConsts.MaxStepDescriptionLength
        );
    }

    public void SetOrder(int order)
    {
        if (order < 1)
        {
            throw new BusinessException(WorkflowErrorCodes.ServiceWorkflows.InvalidStepOrder);
        }

        Order = order;
    }
}
