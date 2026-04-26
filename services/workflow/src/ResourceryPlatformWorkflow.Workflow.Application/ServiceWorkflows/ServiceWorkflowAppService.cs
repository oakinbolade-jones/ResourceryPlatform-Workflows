using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ResourceryPlatformWorkflow.Workflow.Permissions;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

[Authorize(WorkflowPermissions.ServiceWorkflows.Default)]
public class ServiceWorkflowAppService(
    IRepository<ServiceWorkflow, Guid> serviceWorkflowRepository,
    ServiceWorkflowManager serviceWorkflowManager
) : WorkflowAppService,
        IServiceWorkflowAppService
{
    private readonly IRepository<ServiceWorkflow, Guid> _serviceWorkflowRepository =
        serviceWorkflowRepository;
    private readonly ServiceWorkflowManager _serviceWorkflowManager = serviceWorkflowManager;

    public async Task<ServiceWorkflowDto> GetAsync(Guid id)
    {
        var entity = await _serviceWorkflowRepository.GetAsync(id, includeDetails: true);
        return Map(entity);
    }

    public async Task<List<ServiceWorkflowDto>> GetListAsync()
    {
        var entities = await _serviceWorkflowRepository.GetListAsync(includeDetails: true);
        return entities.Select(Map).ToList();
    }

    [Authorize(WorkflowPermissions.ServiceWorkflows.Create)]
    public async Task<ServiceWorkflowDto> CreateAsync(CreateUpdateServiceWorkflowDto input)
    {
        Check.NotNull(input, nameof(input));

        var steps = input.Steps.OrderBy(x => x.Order).Select(step =>
            new ServiceWorkflowStep(
                GuidGenerator.Create(),
                Guid.Empty,
                step.Name,
                step.Code,
                step.Description,
                step.DisplayName,
                step.DisplayNameOutput,
                step.Output,
                step.TATType,
                step.TATUnit,
                step.Order
            )
        );

        var entity = await _serviceWorkflowManager.CreateAsync(
            GuidGenerator.Create(),
            input.Name,
            input.Code,
            input.DisplayName,
            input.LeadTime,
            input.LeadTimeType,
            input.Description,
            input.IsActive,
            steps
        );

        entity = await _serviceWorkflowRepository.GetAsync(entity.Id, includeDetails: true);

        return Map(entity);
    }

    [Authorize(WorkflowPermissions.ServiceWorkflows.Update)]
    public async Task<ServiceWorkflowDto> UpdateAsync(Guid id, CreateUpdateServiceWorkflowDto input)
    {
        Check.NotNull(input, nameof(input));

        var steps = input.Steps.OrderBy(x => x.Order).Select(step =>
            new ServiceWorkflowStep(
                GuidGenerator.Create(),
                id,
                step.Name,
                step.Code,
                step.Description,
                step.DisplayName,
                step.DisplayNameOutput,
                step.Output,
                step.TATType,
                step.TATUnit,
                step.Order
            )
        );

        var entity = await _serviceWorkflowManager.UpdateAsync(
            id,
            input.Name,
            input.Code,
            input.DisplayName,
            input.LeadTime,
            input.LeadTimeType,
            input.Description,
            input.IsActive,
            steps
        );

        entity = await _serviceWorkflowRepository.GetAsync(entity.Id, includeDetails: true);

        return Map(entity);
    }

    [Authorize(WorkflowPermissions.ServiceWorkflows.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _serviceWorkflowManager.DeleteAsync(id);
    }

    private static ServiceWorkflowDto Map(ServiceWorkflow entity)
    {
        return new ServiceWorkflowDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Code = entity.Code,
            DisplayName = entity.DisplayName,
            LeadTime = entity.LeadTime,
            LeadTimeType = entity.LeadTimeType,
            Description = entity.Description,
            IsActive = entity.IsActive,
            CreationTime = entity.CreationTime,
            CreatorId = entity.CreatorId,
            LastModificationTime = entity.LastModificationTime,
            LastModifierId = entity.LastModifierId,
            IsDeleted = entity.IsDeleted,
            DeleterId = entity.DeleterId,
            DeletionTime = entity.DeletionTime,
            Steps = entity
                .Steps.OrderBy(x => x.Order)
                .Select(x => new ServiceWorkflowStepDto
                {
                    Id = x.Id,
                    ServiceWorkflowId = x.ServiceWorkflowId,
                    Name = x.Name,
                    Description = x.Description,
                    Order = x.Order
                })
                .ToList()
        };
    }
}
