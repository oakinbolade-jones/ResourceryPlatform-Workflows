using System;
using Volo.Abp.Application.Dtos;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

public class ServiceWorkflowTaskDto : FullAuditedEntityDto<Guid>
{
    public Guid ServiceWorkflowInstanceId { get; set; }
    public Guid ServiceWorkflowStepId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Guid? AssigneeUserId { get; set; }
    public ServiceWorkflowTaskStatus Status { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
}
