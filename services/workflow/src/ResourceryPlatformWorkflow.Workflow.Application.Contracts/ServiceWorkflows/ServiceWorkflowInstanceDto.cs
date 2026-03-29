using System;
using Volo.Abp.Application.Dtos;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

public class ServiceWorkflowInstanceDto : FullAuditedEntityDto<Guid>
{
    public Guid ServiceWorkflowId { get; set; }
    public Guid RequestId { get; set; }
    public Guid? CurrentStepId { get; set; }
    public ServiceWorkflowInstanceStatus Status { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
