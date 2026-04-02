using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ResourceryPlatformWorkflow.Workflow.Permissions;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace ResourceryPlatformWorkflow.Workflow.Services;

[Authorize(WorkflowPermissions.Services.Default)]
public class ServiceAppService(
    IRepository<Service, Guid> serviceRepository,
    IRepository<ServiceCenter, Guid> serviceCenterRepository,
    IRepository<ServiceWorkflows.ServiceWorkflow, Guid> serviceWorkflowRepository
) : WorkflowAppService, IServiceAppService
{
    private readonly IRepository<Service, Guid> _serviceRepository = serviceRepository;
    private readonly IRepository<ServiceCenter, Guid> _serviceCenterRepository = serviceCenterRepository;
    private readonly IRepository<ServiceWorkflows.ServiceWorkflow, Guid> _serviceWorkflowRepository =
        serviceWorkflowRepository;

    public async Task<ServiceDto> GetAsync(Guid id)
    {
        var entity = await GetServiceWithDetailsAsync(id);
        return await MapAsync(entity);
    }

    public async Task<List<ServiceDto>> GetListAsync()
    {
        var serviceQueryable = await _serviceRepository.WithDetailsAsync(x => x.ServiceCenter);
        var entities = await AsyncExecuter.ToListAsync(serviceQueryable);
        var workflowQueryable = await _serviceWorkflowRepository.GetQueryableAsync();
        var relations = await AsyncExecuter.ToListAsync(
            workflowQueryable
            .Where(x => x.ServiceRelation != null)
            .Select(x => x.ServiceRelation)
            .AsNoTracking()
        );

        var relationsByServiceId = relations
            .GroupBy(x => x.ServiceId)
            .ToDictionary(g => g.Key, g => g.ToList());

        return entities.Select(entity => Map(entity, relationsByServiceId)).ToList();
    }

    [Authorize(WorkflowPermissions.Services.Create)]
    public async Task<ServiceDto> CreateAsync(CreateUpdateServiceDto input)
    {
        Check.NotNull(input, nameof(input));
        await EnsureServiceCenterAvailableAsync(input.ServiceCenterId);

        var entity = new Service(
            GuidGenerator.Create(),
            input.ServiceCenterId,
            input.Name,
            input.Description
        );
        entity.SetIsActive(input.IsActive);

        entity = await _serviceRepository.InsertAsync(entity, autoSave: true);
        await AttachWorkflowsAsync(entity.Id, input.ServiceWorkflowIds);

        entity = await GetServiceWithDetailsAsync(entity.Id);
        return await MapAsync(entity);
    }

    [Authorize(WorkflowPermissions.Services.Update)]
    public async Task<ServiceDto> UpdateAsync(Guid id, CreateUpdateServiceDto input)
    {
        Check.NotNull(input, nameof(input));
        await EnsureServiceCenterAvailableAsync(input.ServiceCenterId, id);

        var entity = await GetServiceWithDetailsAsync(id);
        entity.SetServiceCenter(input.ServiceCenterId);
        entity.SetName(input.Name);
        entity.SetDescription(input.Description);
        entity.SetIsActive(input.IsActive);

        entity = await _serviceRepository.UpdateAsync(entity, autoSave: true);
        await AttachWorkflowsAsync(entity.Id, input.ServiceWorkflowIds);

        entity = await GetServiceWithDetailsAsync(entity.Id);
        return await MapAsync(entity);
    }

    [Authorize(WorkflowPermissions.Services.Delete)]
    public Task DeleteAsync(Guid id) => _serviceRepository.DeleteAsync(id, autoSave: true);

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
        var relations = await AsyncExecuter.ToListAsync(
            workflowQueryable
                .Where(x => x.ServiceRelation != null && x.ServiceRelation.ServiceId == entity.Id)
                .Select(x => x.ServiceRelation)
                .AsNoTracking()
        );

        return Map(entity, new Dictionary<Guid, List<ServiceRelation>> { [entity.Id] = relations });
    }

    private async Task<Service> GetServiceWithDetailsAsync(Guid id)
    {
        var queryable = await _serviceRepository.WithDetailsAsync(x => x.ServiceCenter);
        var entity = await AsyncExecuter.FirstOrDefaultAsync(queryable, x => x.Id == id);

        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(Service), id);
        }

        return entity;
    }

    private async Task EnsureServiceCenterAvailableAsync(
        Guid serviceCenterId,
        Guid? excludingServiceId = null
    )
    {
        if (serviceCenterId == Guid.Empty)
        {
            throw new BusinessException("Workflow:Services:ServiceCenterRequired");
        }

        await _serviceCenterRepository.GetAsync(serviceCenterId);

        var queryable = await _serviceRepository.GetQueryableAsync();
        var isAlreadyAssigned = await AsyncExecuter.AnyAsync(
            queryable,
            x => x.ServiceCenterId == serviceCenterId && (!excludingServiceId.HasValue || x.Id != excludingServiceId.Value)
        );

        if (isAlreadyAssigned)
        {
            throw new BusinessException("Workflow:Services:ServiceCenterAlreadyAssigned");
        }
    }

    private static ServiceDto Map(Service entity, IReadOnlyDictionary<Guid, List<ServiceRelation>> relationsByServiceId)
    {
        relationsByServiceId.TryGetValue(entity.Id, out var relations);
        relations ??= new List<ServiceRelation>();

        return new ServiceDto
        {
            Id = entity.Id,
            ServiceCenterId = entity.ServiceCenterId,
            Name = entity.Name,
            Description = entity.Description,
            IsActive = entity.IsActive,
            ServiceCenter = entity.ServiceCenter == null
                ? null
                : new ServiceCenterDto
                {
                    Id = entity.ServiceCenter.Id,
                    Name = entity.ServiceCenter.Name,
                    DisplayName = entity.ServiceCenter.DisplayName,
                    Description = entity.ServiceCenter.Description,
                    Code = entity.ServiceCenter.Code,
                    CreationTime = entity.ServiceCenter.CreationTime,
                    CreatorId = entity.ServiceCenter.CreatorId,
                    LastModificationTime = entity.ServiceCenter.LastModificationTime,
                    LastModifierId = entity.ServiceCenter.LastModifierId,
                    IsDeleted = entity.ServiceCenter.IsDeleted,
                    DeleterId = entity.ServiceCenter.DeleterId,
                    DeletionTime = entity.ServiceCenter.DeletionTime,
                },
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
