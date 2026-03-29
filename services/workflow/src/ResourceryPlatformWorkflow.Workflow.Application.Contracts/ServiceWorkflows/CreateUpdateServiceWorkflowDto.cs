using System;
using System.Collections.Generic;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

public class CreateUpdateServiceWorkflowDto
{
    public Guid ServiceId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; } = true;
    public IList<CreateUpdateServiceWorkflowStepDto> Steps { get; set; } =
        new List<CreateUpdateServiceWorkflowStepDto>();
}
