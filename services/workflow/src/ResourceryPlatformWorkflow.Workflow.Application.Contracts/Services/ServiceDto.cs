using System;
using Volo.Abp.Application.Dtos;

namespace ResourceryPlatformWorkflow.Workflow.Services;

public class ServiceDto : FullAuditedEntityDto<Guid>
{
    public Guid ServiceCenterId { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
}
