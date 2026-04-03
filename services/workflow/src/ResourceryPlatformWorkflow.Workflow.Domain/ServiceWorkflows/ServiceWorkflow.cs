using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

public class ServiceWorkflow : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; private set; }
    public string Name { get; private set; }
    public string Code { get; private set; }
    public string DisplayName { get; private set; }
    public string LeadTime { get; private set; }
    public string LeadTimeType { get; private set; }
    public string Description { get; private set; }
    public bool IsActive { get; private set; }
    public ICollection<ServiceWorkflowStep> Steps { get; private set; }

    protected ServiceWorkflow()
    {
        Steps = new List<ServiceWorkflowStep>();
    }

    public ServiceWorkflow(Guid id, string name, string code, string displayName, string leadTime, string leadTimeType, string description)
        : base(id)
    {
        Steps = new List<ServiceWorkflowStep>();
        SetName(name);
        SetCode(code);
        SetDisplayName(displayName);
        SetLeadTime(leadTime);
        SetLeadTimeType(leadTimeType);
        SetDescription(description);
        IsActive = true;
    }

    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), ServiceWorkflowConsts.MaxWorkflowNameLength);
    }

    public void SetDisplayName(string displayName)
    {
        DisplayName = Check.NotNullOrWhiteSpace(
            displayName,
            nameof(displayName),
            ServiceWorkflowConsts.MaxWorkflowDisplayNameLength
        );
    }

    public void SetCode(string code)
    {
        Code = Check.NotNullOrWhiteSpace(
            code,
            nameof(code),
            ServiceWorkflowConsts.MaxWorkflowCodeLength
        );
    }

    public void SetLeadTime(string leadTime)
    {
        LeadTime = Check.NotNullOrWhiteSpace(
            leadTime,
            nameof(leadTime),
            ServiceWorkflowConsts.MaxLeadTimeLength
        );
    }

    public void SetLeadTimeType(string leadTimeType)
    {
        LeadTimeType = Check.NotNullOrWhiteSpace(
            leadTimeType,
            nameof(leadTimeType),
            ServiceWorkflowConsts.MaxLeadTimeTypeLength
        );
    }

    public void SetDescription(string description)
    {
        Description = Check.NotNullOrWhiteSpace(
            description,
            nameof(description),
            ServiceWorkflowConsts.MaxWorkflowDescriptionLength
        );
    }

    public void SetIsActive(bool isActive)
    {
        IsActive = isActive;
    }

    public void AddStep(Guid stepId, string name, string code, string description, int order, string displayName = null, string displayNameOutput = null, string output = null, string tatType = null, string tatUnit = null)
    {
        Check.NotNull(stepId, nameof(stepId));

        if (order < 1)
        {
            throw new BusinessException(WorkflowErrorCodes.ServiceWorkflows.InvalidStepOrder);
        }

        if (Steps.Any(x => x.Order == order))
        {
            throw new BusinessException(WorkflowErrorCodes.ServiceWorkflows.DuplicateStepOrder);
        }

        var effectiveDisplayName = string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        var effectiveDisplayNameOutput = string.IsNullOrWhiteSpace(displayNameOutput) ? name : displayNameOutput;
        var effectiveOutput = string.IsNullOrWhiteSpace(output) ? string.Empty : output;
        var effectiveCode = string.IsNullOrWhiteSpace(code) ? name : code;
        var effectiveTATType = string.IsNullOrWhiteSpace(tatType) ? "Value" : tatType;
        var effectiveTATUnit = string.IsNullOrWhiteSpace(tatUnit) ? "Minutes" : tatUnit;

        Steps.Add(new ServiceWorkflowStep(
            stepId,
            Id,
            name,
            effectiveCode,
            description,
            effectiveDisplayName,
            effectiveDisplayNameOutput,
            effectiveOutput,
            effectiveTATType,
            effectiveTATUnit,
            order));
    }
}
