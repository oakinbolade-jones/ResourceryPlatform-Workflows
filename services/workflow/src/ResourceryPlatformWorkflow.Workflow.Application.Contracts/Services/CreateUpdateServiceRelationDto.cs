using System;

namespace ResourceryPlatformWorkflow.Workflow.Services;

public class CreateUpdateServiceRelationDto
{
    public Guid ServiceId { get; set; }
    public Guid ServiceWorkflowId { get; set; }
}
