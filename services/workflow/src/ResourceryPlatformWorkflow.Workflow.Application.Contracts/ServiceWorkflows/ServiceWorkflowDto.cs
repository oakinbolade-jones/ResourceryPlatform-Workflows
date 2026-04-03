using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

public class ServiceWorkflowDto : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string DisplayName { get; set; }
    public string LeadTime { get; set; }
    public string LeadTimeType { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public IList<ServiceWorkflowStepDto> Steps { get; set; } = new List<ServiceWorkflowStepDto>();
}
