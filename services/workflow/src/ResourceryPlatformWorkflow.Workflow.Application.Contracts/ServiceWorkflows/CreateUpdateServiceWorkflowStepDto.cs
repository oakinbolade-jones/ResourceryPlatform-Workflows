using System;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

public class CreateUpdateServiceWorkflowStepDto
{
    public Guid ServiceWorkflowId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Order { get; set; }
}
