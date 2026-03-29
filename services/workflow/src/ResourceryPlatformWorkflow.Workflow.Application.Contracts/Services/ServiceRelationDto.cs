using System;

namespace ResourceryPlatformWorkflow.Workflow.Services;

public class ServiceRelationDto
{
    public Guid ServiceId { get; set; }
    public Guid ServiceWorkflowId { get; set; }
}
