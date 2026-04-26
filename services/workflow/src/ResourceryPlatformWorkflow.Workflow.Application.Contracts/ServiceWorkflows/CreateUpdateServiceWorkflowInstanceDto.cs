using System;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

public class CreateUpdateServiceWorkflowInstanceDto
{
    public Guid ServiceWorkflowId { get; set; }
    public Guid RequestId { get; set; }
    public Guid? CurrentStepId { get; set; }
    public ServiceWorkflowInstanceStatus Status { get; set; } = ServiceWorkflowInstanceStatus.Pending;
}
