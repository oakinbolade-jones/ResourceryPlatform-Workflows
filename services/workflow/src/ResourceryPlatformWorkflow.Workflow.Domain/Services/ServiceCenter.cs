using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace ResourceryPlatformWorkflow.Workflow.Services;

public class ServiceCenter : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; private set; }
    public string Name { get; private set; }
    public string DisplayName { get; private set; }
    public string Description { get; private set; }
    public string Code { get; private set; }

    protected ServiceCenter() { }

    public ServiceCenter(Guid id, string name, string displayName, string description, string code)
        : base(id)
    {
        SetName(name);
        SetDisplayName(displayName);
        SetDescription(description);
        SetCode(code);
    }

    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(
            name,
            nameof(name),
            ServiceCenterConsts.MaxServiceCenterNameLength
        );
    }

    public void SetDisplayName(string displayName)
    {
        DisplayName = Check.NotNullOrWhiteSpace(
            displayName,
            nameof(displayName),
            ServiceCenterConsts.MaxServiceCenterDisplayNameLength
        );
    }

    public void SetDescription(string description)
    {
        Description = Check.NotNullOrWhiteSpace(
            description,
            nameof(description),
            ServiceCenterConsts.MaxServiceCenterDescriptionLength
        );
    }

    public void SetCode(string code)
    {
        Code = Check.NotNullOrWhiteSpace(
            code,
            nameof(code),
            ServiceCenterConsts.MaxServiceCenterCodeLength
        );
    }
}