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
public class ServiceWorkflowAppService(IRepository<ServiceWorkflow, Guid> serviceWorkflowRepository)
    : WorkflowAppService,
        IServiceWorkflowAppService
{
    private readonly IRepository<ServiceWorkflow, Guid> _serviceWorkflowRepository =
        serviceWorkflowRepository;

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

        var entity = new ServiceWorkflow(
            GuidGenerator.Create(),
            input.ServiceId,
            input.Name,
            input.Description
        );
        entity.SetIsActive(input.IsActive);

        foreach (var step in input.Steps.OrderBy(x => x.Order))
        {
            entity.AddStep(GuidGenerator.Create(), step.Name, step.Description, step.Order);
        }

        entity = await _serviceWorkflowRepository.InsertAsync(entity, autoSave: true);
        entity = await _serviceWorkflowRepository.GetAsync(entity.Id, includeDetails: true);

        return Map(entity);
    }

    [Authorize(WorkflowPermissions.ServiceWorkflows.Update)]
    public async Task<ServiceWorkflowDto> UpdateAsync(Guid id, CreateUpdateServiceWorkflowDto input)
    {
        Check.NotNull(input, nameof(input));

        var entity = await _serviceWorkflowRepository.GetAsync(id, includeDetails: true);

        entity.SetService(input.ServiceId);
        entity.SetName(input.Name);
        entity.SetDescription(input.Description);
        entity.SetIsActive(input.IsActive);

        entity.Steps.Clear();
        foreach (var step in input.Steps.OrderBy(x => x.Order))
        {
            entity.AddStep(GuidGenerator.Create(), step.Name, step.Description, step.Order);
        }

        entity = await _serviceWorkflowRepository.UpdateAsync(entity, autoSave: true);
        entity = await _serviceWorkflowRepository.GetAsync(entity.Id, includeDetails: true);

        return Map(entity);
    }

    [Authorize(WorkflowPermissions.ServiceWorkflows.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _serviceWorkflowRepository.DeleteAsync(id, autoSave: true);
    }

    private static ServiceWorkflowDto Map(ServiceWorkflow entity)
    {
        return new ServiceWorkflowDto
        {
            Id = entity.Id,
            ServiceId = entity.ServiceRelation?.ServiceId ?? Guid.Empty,
            RelationServiceId = entity.ServiceRelation?.ServiceId ?? Guid.Empty,
            RelationServiceWorkflowId = entity.ServiceRelation?.ServiceWorkflowId ?? entity.Id,
            Name = entity.Name,
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
