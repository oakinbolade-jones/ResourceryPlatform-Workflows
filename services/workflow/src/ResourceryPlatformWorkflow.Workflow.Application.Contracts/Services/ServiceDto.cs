using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace ResourceryPlatformWorkflow.Workflow.Services;

public class ServiceDto : FullAuditedEntityDto<Guid>
{
    public Guid ServiceCenterId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public ServiceCenterDto ServiceCenter { get; set; }
    public IList<ServiceRelationDto> Relations { get; set; } = new List<ServiceRelationDto>();
}
