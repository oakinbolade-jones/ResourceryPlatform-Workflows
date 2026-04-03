using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

public class ServiceWorkflowStep : FullAuditedEntity<Guid>, IMultiTenant
{
    public Guid? TenantId { get; private set; }
    public Guid ServiceWorkflowId { get; private set; }
    public string Name { get; private set; }
    public string Code { get; private set; }
    public string Description { get; private set; }
    public string DisplayName { get; private set; }
    public string DisplayNameOutput { get; private set; }
    public string Output { get; private set; }
    public string TATType { get; private set; }
    public string TATUnit { get; private set; }
    public int Order { get; private set; }

    protected ServiceWorkflowStep() { }

    public ServiceWorkflowStep(Guid id, Guid serviceWorkflowId, string name, string code, string description, string displayName, string displayNameOutput, string output, string tatType, string tatUnit, int order)
        : base(id)
    {
        ServiceWorkflowId = serviceWorkflowId;
        SetName(name);
        SetCode(code);
        SetDescription(description);
        SetDisplayName(displayName);
        SetDisplayNameOutput(displayNameOutput);
        SetOutput(output);
        SetTATType(tatType);
        SetTATUnit(tatUnit);
        SetOrder(order);
    }

    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), ServiceWorkflowConsts.MaxStepNameLength);
    }

    public void SetCode(string code)
    {
        Code = Check.NotNullOrWhiteSpace(code, nameof(code), ServiceWorkflowConsts.MaxStepCodeLength);
    }

    public void SetDescription(string description)
    {
        Description = Check.NotNullOrWhiteSpace(
            description,
            nameof(description),
            ServiceWorkflowConsts.MaxStepDescriptionLength
        );
    }

    public void SetDisplayName(string displayName)
    {
        DisplayName = Check.NotNullOrWhiteSpace(
            displayName,
            nameof(displayName),
            ServiceWorkflowConsts.MaxStepDisplayNameProcessLength
        );
    }

    public void SetDisplayNameOutput(string displayNameOutput)
    {
        DisplayNameOutput = Check.NotNullOrWhiteSpace(
            displayNameOutput,
            nameof(displayNameOutput),
            ServiceWorkflowConsts.MaxStepDisplayNameExpectedOutputLength
        );
    }

    public void SetOutput(string output)
    {
        Output = Check.NotNullOrWhiteSpace(
            output,
            nameof(output),
            ServiceWorkflowConsts.MaxStepOutputLength
        );
    }

    public void SetTATType(string tatType)
    {
        TATType = Check.NotNullOrWhiteSpace(
            tatType,
            nameof(tatType),
            ServiceWorkflowConsts.MaxStepTATTypeLength
        );
    }

    public void SetTATUnit(string tatUnit)
    {
        TATUnit = Check.NotNullOrWhiteSpace(
            tatUnit,
            nameof(tatUnit),
            ServiceWorkflowConsts.MaxStepTATUnitLength
        );
    }

    public void SetOrder(int order)
    {
        if (order < 1)
        {
            throw new BusinessException(WorkflowErrorCodes.ServiceWorkflows.InvalidStepOrder);
        }

        Order = order;
    }
}
