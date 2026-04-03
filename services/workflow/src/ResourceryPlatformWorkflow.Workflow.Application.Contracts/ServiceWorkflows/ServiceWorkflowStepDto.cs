using System;
using Volo.Abp.Application.Dtos;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

public class ServiceWorkflowStepDto : EntityDto<Guid>
{
    public Guid ServiceWorkflowId { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public string DisplayName { get; set; }
    public string DisplayNameOutput { get; set; }
    public string Output { get; set; }
    public string TATType { get; set; }
    public string TATUnit { get; set; }
    public int Order { get; set; }
}
