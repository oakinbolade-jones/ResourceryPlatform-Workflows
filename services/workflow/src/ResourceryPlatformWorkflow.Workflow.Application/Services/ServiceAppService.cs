using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ResourceryPlatformWorkflow.Workflow.Permissions;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace ResourceryPlatformWorkflow.Workflow.Services;

[Authorize(WorkflowPermissions.Services.Default)]
public class ServiceAppService(
    IRepository<Service, Guid> serviceRepository,
    IRepository<ServiceWorkflows.ServiceWorkflow, Guid> serviceWorkflowRepository
) : WorkflowAppService, IServiceAppService
{
    private readonly IRepository<Service, Guid> _serviceRepository = serviceRepository;
    private readonly IRepository<ServiceWorkflows.ServiceWorkflow, Guid> _serviceWorkflowRepository =
        serviceWorkflowRepository;

    public async Task<ServiceDto> GetAsync(Guid id)
    {
        var entity = await _serviceRepository.GetAsync(id, includeDetails: true);
        return await MapAsync(entity);
    }

    public async Task<List<ServiceDto>> GetListAsync()
    {
        var entities = await _serviceRepository.GetListAsync(includeDetails: true);
        var workflowQueryable = await _serviceWorkflowRepository.GetQueryableAsync();
        var relationsByServiceId = workflowQueryable
            .Where(x => x.ServiceRelation != null)
            .Select(x => x.ServiceRelation)
            .ToList()
            .GroupBy(x => x.ServiceId)
            .ToDictionary(g => g.Key, g => g.ToList());

        return entities.Select(entity => Map(entity, relationsByServiceId)).ToList();
    }

    [Authorize(WorkflowPermissions.Services.Create)]
    public async Task<ServiceDto> CreateAsync(CreateUpdateServiceDto input)
    {
        Check.NotNull(input, nameof(input));

        var entity = new Service(GuidGenerator.Create(), input.Name, input.Description);
        entity.SetIsActive(input.IsActive);

        entity = await _serviceRepository.InsertAsync(entity, autoSave: true);
        await AttachWorkflowsAsync(entity.Id, input.ServiceWorkflowIds);

        entity = await _serviceRepository.GetAsync(entity.Id, includeDetails: true);
        return await MapAsync(entity);
    }

    [Authorize(WorkflowPermissions.Services.Update)]
    public async Task<ServiceDto> UpdateAsync(Guid id, CreateUpdateServiceDto input)
    {
        Check.NotNull(input, nameof(input));

        var entity = await _serviceRepository.GetAsync(id, includeDetails: true);
        entity.SetName(input.Name);
        entity.SetDescription(input.Description);
        entity.SetIsActive(input.IsActive);

        entity = await _serviceRepository.UpdateAsync(entity, autoSave: true);
        await AttachWorkflowsAsync(entity.Id, input.ServiceWorkflowIds);

        entity = await _serviceRepository.GetAsync(entity.Id, includeDetails: true);
        return await MapAsync(entity);
    }

    [Authorize(WorkflowPermissions.Services.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _serviceRepository.DeleteAsync(id, autoSave: true);
    }

    private async Task AttachWorkflowsAsync(Guid serviceId, IEnumerable<Guid> workflowIds)
    {
        if (workflowIds == null)
        {
            return;
        }

        foreach (var workflowId in workflowIds.Distinct())
        {
            var workflow = await _serviceWorkflowRepository.GetAsync(workflowId);
            workflow.SetService(serviceId);
            await _serviceWorkflowRepository.UpdateAsync(workflow, autoSave: true);
        }
    }

    private async Task<ServiceDto> MapAsync(Service entity)
    {
        var workflowQueryable = await _serviceWorkflowRepository.GetQueryableAsync();
        var relations = workflowQueryable
            .Where(x => x.ServiceRelation != null && x.ServiceRelation.ServiceId == entity.Id)
            .Select(x => x.ServiceRelation)
            .ToList();

        return Map(entity, new Dictionary<Guid, List<ServiceRelation>> { [entity.Id] = relations });
    }

    private static ServiceDto Map(Service entity, IReadOnlyDictionary<Guid, List<ServiceRelation>> relationsByServiceId)
    {
        relationsByServiceId.TryGetValue(entity.Id, out var relations);
        relations ??= new List<ServiceRelation>();

        return new ServiceDto
        {
            Id = entity.Id,
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
            Relations = relations
                .Select(x => new ServiceRelationDto
                {
                    ServiceId = x.ServiceId,
                    ServiceWorkflowId = x.ServiceWorkflowId
                })
                .ToList()
        };
    }
}
