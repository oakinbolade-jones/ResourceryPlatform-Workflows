using System;
using System.ComponentModel;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace ResourceryPlatformWorkflow.Workflow.Services;

public class Service : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; private set; }
    public Guid ServiceCenterId { get; private set; }
    public string Name { get; private set; }
    public string DisplayName { get; private set; }
    public string Description { get; private set; }
    public bool IsActive { get; private set; }
    protected Service() { }

    public Service(Guid id, Guid serviceCenterId, string name, string displayName, string description)
        : base(id)
    {
        SetServiceCenter(serviceCenterId);
        SetName(name);
        SetDisplayName(displayName);
        SetDescription(description);
        IsActive = true;
    }

    public void SetServiceCenter(Guid serviceCenterId)
    {
        if (serviceCenterId == Guid.Empty)
        {
            throw new ArgumentException("Service center id cannot be empty.", nameof(serviceCenterId));
        }

        ServiceCenterId = serviceCenterId;
    }

    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), ServiceConsts.MaxServiceNameLength);
    }

    public void SetDisplayName(string displayName)
    {
        DisplayName = Check.NotNullOrWhiteSpace(
            displayName,
            nameof(displayName),
            ServiceConsts.MaxServiceDisplayNameLength
        );
    }

    public void SetDescription(string description)
    {
        Description = Check.NotNullOrWhiteSpace(
            description,
            nameof(description),
            ServiceConsts.MaxServiceDescriptionLength
        );
    }

    public void SetIsActive(bool isActive)
    {
        IsActive = isActive;
    }
}
