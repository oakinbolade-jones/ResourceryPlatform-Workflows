namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

public enum ServiceWorkflowInstanceStatus
{
    Pending = 0,
    InProgress = 1,
    Completed = 2,
    Rejected = 3,
    Cancelled = 4
}
