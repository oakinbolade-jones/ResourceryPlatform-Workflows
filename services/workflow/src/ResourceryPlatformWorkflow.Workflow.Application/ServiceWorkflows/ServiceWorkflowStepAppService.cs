using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ResourceryPlatformWorkflow.Workflow.Permissions;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

[Authorize(WorkflowPermissions.ServiceWorkflowSteps.Default)]
public class ServiceWorkflowStepAppService(
    IRepository<ServiceWorkflowStep, Guid> serviceWorkflowStepRepository,
    IRepository<ServiceWorkflow, Guid> serviceWorkflowRepository
) : WorkflowAppService, IServiceWorkflowStepAppService
{
    private readonly IRepository<ServiceWorkflowStep, Guid> _serviceWorkflowStepRepository =
        serviceWorkflowStepRepository;
    private readonly IRepository<ServiceWorkflow, Guid> _serviceWorkflowRepository =
        serviceWorkflowRepository;

    public async Task<ServiceWorkflowStepDto> GetAsync(Guid id)
    {
        var entity = await _serviceWorkflowStepRepository.GetAsync(id);
        return Map(entity);
    }

    public async Task<List<ServiceWorkflowStepDto>> GetListAsync()
    {
        var entities = await _serviceWorkflowStepRepository.GetListAsync();
        return entities.OrderBy(x => x.Order).Select(Map).ToList();
    }

    [Authorize(WorkflowPermissions.ServiceWorkflowSteps.Create)]
    public async Task<ServiceWorkflowStepDto> CreateAsync(CreateUpdateServiceWorkflowStepDto input)
    {
        Check.NotNull(input, nameof(input));

        var workflow = await _serviceWorkflowRepository.GetAsync(input.ServiceWorkflowId, includeDetails: true);
        workflow.AddStep(GuidGenerator.Create(), input.Name, input.Description, input.Order);

        await _serviceWorkflowRepository.UpdateAsync(workflow, autoSave: true);

        var created = workflow.Steps.Single(x => x.Order == input.Order && x.Name == input.Name);
        return Map(created);
    }

    [Authorize(WorkflowPermissions.ServiceWorkflowSteps.Update)]
    public async Task<ServiceWorkflowStepDto> UpdateAsync(Guid id, CreateUpdateServiceWorkflowStepDto input)
    {
        Check.NotNull(input, nameof(input));

        var entity = await _serviceWorkflowStepRepository.GetAsync(id);
        entity.SetName(input.Name);
        entity.SetDescription(input.Description);
        entity.SetOrder(input.Order);

        entity = await _serviceWorkflowStepRepository.UpdateAsync(entity, autoSave: true);
        return Map(entity);
    }

    [Authorize(WorkflowPermissions.ServiceWorkflowSteps.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _serviceWorkflowStepRepository.DeleteAsync(id, autoSave: true);
    }

    private static ServiceWorkflowStepDto Map(ServiceWorkflowStep entity)
    {
        return new ServiceWorkflowStepDto
        {
            Id = entity.Id,
            ServiceWorkflowId = entity.ServiceWorkflowId,
            Name = entity.Name,
            Description = entity.Description,
            Order = entity.Order
        };
    }
}
