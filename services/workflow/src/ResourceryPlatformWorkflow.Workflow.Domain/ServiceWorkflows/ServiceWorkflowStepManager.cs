using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

public class ServiceWorkflowStepManager : DomainService
{
    private readonly IRepository<ServiceWorkflowStep, Guid> _serviceWorkflowStepRepository;
    private readonly IRepository<ServiceWorkflow, Guid> _serviceWorkflowRepository;

    public ServiceWorkflowStepManager(
        IRepository<ServiceWorkflowStep, Guid> serviceWorkflowStepRepository,
        IRepository<ServiceWorkflow, Guid> serviceWorkflowRepository
    )
    {
        _serviceWorkflowStepRepository = serviceWorkflowStepRepository;
        _serviceWorkflowRepository = serviceWorkflowRepository;
    }

    public async Task<ServiceWorkflowStep> CreateAsync(
        Guid serviceWorkflowId,
        Guid id,
        string name,
        string code,
        string description,
        int order,
        string displayName,
        string displayNameOutput,
        string output,
        string tatType,
        string tatUnit
    )
    {
        var workflow = await _serviceWorkflowRepository.GetAsync(serviceWorkflowId, includeDetails: true);

        workflow.AddStep(id, name, code, description, order, displayName, displayNameOutput, output, tatType, tatUnit);

        await _serviceWorkflowRepository.UpdateAsync(workflow, autoSave: true);

        return await _serviceWorkflowStepRepository.GetAsync(id);
    }

    public async Task<ServiceWorkflowStep> UpdateAsync(
        Guid id,
        string name,
        string code,
        string description,
        int order,
        string displayName,
        string displayNameOutput,
        string output,
        string tatType,
        string tatUnit
    )
    {
        var entity = await _serviceWorkflowStepRepository.GetAsync(id);

        entity.SetName(name);
        entity.SetCode(code);
        entity.SetDescription(description);
        entity.SetDisplayName(displayName);
        entity.SetDisplayNameOutput(displayNameOutput);
        entity.SetOutput(output);
        entity.SetTATType(tatType);
        entity.SetTATUnit(tatUnit);
        entity.SetOrder(order);

        return await _serviceWorkflowStepRepository.UpdateAsync(entity, autoSave: true);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _serviceWorkflowStepRepository.DeleteAsync(id, autoSave: true);
    }
}
