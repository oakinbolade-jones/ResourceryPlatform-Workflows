using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Values;

namespace ResourceryPlatformWorkflow.Workflow.Services;

public class ServiceRelation : ValueObject
{
    public Guid ServiceId { get; private set; }
    public Guid ServiceWorkflowId { get; private set; }

    protected ServiceRelation() { }

    public ServiceRelation(Guid serviceId, Guid serviceWorkflowId)
    {
        ServiceId = Check.NotNull(serviceId, nameof(serviceId));
        ServiceWorkflowId = Check.NotNull(serviceWorkflowId, nameof(serviceWorkflowId));
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return ServiceId;
        yield return ServiceWorkflowId;
    }
}
