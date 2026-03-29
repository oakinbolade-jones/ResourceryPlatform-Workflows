using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

public class ServiceWorkflowDto : FullAuditedEntityDto<Guid>
{
    public Guid ServiceId { get; set; }
    public Guid RelationServiceId { get; set; }
    public Guid RelationServiceWorkflowId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public IList<ServiceWorkflowStepDto> Steps { get; set; } = new List<ServiceWorkflowStepDto>();
}
