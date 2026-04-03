using System;
using System.Collections.Generic;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

public class CreateUpdateServiceWorkflowDto
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string DisplayName { get; set; }
    public string LeadTime { get; set; }
    public string LeadTimeType { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; } = true;
    public IList<CreateUpdateServiceWorkflowStepDto> Steps { get; set; } =
        new List<CreateUpdateServiceWorkflowStepDto>();
}
