using System;
using System.Collections.Generic;

namespace ResourceryPlatformWorkflow.Workflow.Services;

public class CreateUpdateServiceDto
{
    public Guid ServiceCenterId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; } = true;
    public IList<Guid> ServiceWorkflowIds { get; set; } = new List<Guid>();
}
