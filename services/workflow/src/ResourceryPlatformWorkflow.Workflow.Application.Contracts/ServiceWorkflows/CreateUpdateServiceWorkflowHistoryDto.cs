using System;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

public class CreateUpdateServiceWorkflowHistoryDto
{
    public Guid ServiceWorkflowInstanceId { get; set; }
    public Guid? ServiceWorkflowStepId { get; set; }
    public Guid? ServiceWorkflowTaskId { get; set; }
    public ServiceWorkflowHistoryType Type { get; set; }
    public string Action { get; set; }
    public string Comment { get; set; }
    public Guid? PerformedByUserId { get; set; }
}
