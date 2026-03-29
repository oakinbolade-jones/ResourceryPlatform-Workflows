using System;
using Volo.Abp.Application.Dtos;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

public class ServiceWorkflowStepDto : EntityDto<Guid>
{
    public Guid ServiceWorkflowId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Order { get; set; }
}
