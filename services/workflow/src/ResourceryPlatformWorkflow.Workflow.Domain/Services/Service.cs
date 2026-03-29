using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace ResourceryPlatformWorkflow.Workflow.Services;

public class Service : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool IsActive { get; private set; }
    protected Service() { }

    public Service(Guid id, string name, string description)
        : base(id)
    {
        SetName(name);
        SetDescription(description);
        IsActive = true;
    }

    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), ServiceConsts.MaxServiceNameLength);
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
