using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace ResourceryPlatformWorkflow.Workflow.Meetings;

public class MeetingRequirement : Entity<Guid>, IMultiTenant
{
   public Guid? TenantId { get; private set; }
    public string ItemCode { get; set; }    
    public string ItemName { get; set; }
    public string Category { get; set; }
    public string ServiceCenterCode { get; set; }    
    public string DisplayNameItemName {get; set;  }
    public string DisplayNameServiceCenter {get; set;  }
    public string DisplayNameItemCategory {get; set;  }
}