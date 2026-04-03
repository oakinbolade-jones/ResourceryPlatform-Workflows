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
    ServiceWorkflowStepManager serviceWorkflowStepManager
) : WorkflowAppService, IServiceWorkflowStepAppService
{
    private readonly IRepository<ServiceWorkflowStep, Guid> _serviceWorkflowStepRepository =
        serviceWorkflowStepRepository;
    private readonly ServiceWorkflowStepManager _serviceWorkflowStepManager = serviceWorkflowStepManager;

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

        var entity = await _serviceWorkflowStepManager.CreateAsync(
            input.ServiceWorkflowId,
            GuidGenerator.Create(),
            input.Name,
            input.Code,
            input.Description,
            input.Order,
            input.DisplayName,
            input.DisplayNameOutput,
            input.Output,
            input.TATType,
            input.TATUnit
        );

        return Map(entity);
    }

    [Authorize(WorkflowPermissions.ServiceWorkflowSteps.Update)]
    public async Task<ServiceWorkflowStepDto> UpdateAsync(Guid id, CreateUpdateServiceWorkflowStepDto input)
    {
        Check.NotNull(input, nameof(input));

        var entity = await _serviceWorkflowStepManager.UpdateAsync(
            id,
            input.Name,
            input.Code,
            input.Description,
            input.Order,
            input.DisplayName,
            input.DisplayNameOutput,
            input.Output,
            input.TATType,
            input.TATUnit
        );

        return Map(entity);
    }

    [Authorize(WorkflowPermissions.ServiceWorkflowSteps.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _serviceWorkflowStepManager.DeleteAsync(id);
    }

    private static ServiceWorkflowStepDto Map(ServiceWorkflowStep entity)
    {
        return new ServiceWorkflowStepDto
        {
            Id = entity.Id,
            ServiceWorkflowId = entity.ServiceWorkflowId,
            Name = entity.Name,
            Code = entity.Code,
            Description = entity.Description,
            DisplayName = entity.DisplayName,
            DisplayNameOutput = entity.DisplayNameOutput,
            Output = entity.Output,
            TATType = entity.TATType,
            TATUnit = entity.TATUnit,
            Order = entity.Order
        };
    }
}
