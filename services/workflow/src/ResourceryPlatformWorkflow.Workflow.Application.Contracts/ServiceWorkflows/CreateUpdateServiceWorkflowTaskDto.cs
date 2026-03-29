using System;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

public class CreateUpdateServiceWorkflowTaskDto
{
    public Guid ServiceWorkflowInstanceId { get; set; }
    public Guid ServiceWorkflowStepId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Guid? AssigneeUserId { get; set; }
    public ServiceWorkflowTaskStatus Status { get; set; } = ServiceWorkflowTaskStatus.Pending;
    public DateTime? DueDate { get; set; }
}
