
using System;
using Volo.Abp.Application.Dtos;

namespace ResourceryPlatformWorkflow.Workflow.Services;

public class ServiceCenterDto : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public string Code { get; set; }
}