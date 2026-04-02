using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
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
        var entity = await GetServiceWithDetailsAsync(id);
        return await MapAsync(entity);
    }

    public async Task<List<ServiceDto>> GetListAsync()
    {
        var serviceQueryable = await _serviceRepository.GetQueryableAsync();
        var entities = await AsyncExecuter.ToListAsync(serviceQueryable);

        return entities.Select(Map).ToList();
    }

    [Authorize(WorkflowPermissions.Services.Create)]
    public async Task<ServiceDto> CreateAsync(CreateUpdateServiceDto input)
    {
        Check.NotNull(input, nameof(input));

        var entity = new Service(
            GuidGenerator.Create(),
            input.ServiceCenterId,
            input.Name,
            input.DisplayName,
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

        var entity = await GetServiceWithDetailsAsync(id);
        entity.SetServiceCenter(input.ServiceCenterId);
        entity.SetName(input.Name);
        entity.SetDisplayName(input.DisplayName);
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
        return await Task.FromResult(Map(entity));
    }

    private async Task<Service> GetServiceWithDetailsAsync(Guid id)
    {
        var queryable = await _serviceRepository.GetQueryableAsync();
        var entity = await AsyncExecuter.FirstOrDefaultAsync(queryable, x => x.Id == id);

        if (entity == null)
        {
            throw new BusinessException(WorkflowErrorCodes.Services.ServiceNotFound);
        }

        return entity;
    }

    private static ServiceDto Map(Service entity)
    {
        return new ServiceDto
        {
            Id = entity.Id,
            ServiceCenterId = entity.ServiceCenterId,
            Name = entity.Name,
            DisplayName = entity.DisplayName,
            Description = entity.Description,
            IsActive = entity.IsActive,
            CreationTime = entity.CreationTime,
            CreatorId = entity.CreatorId,
            LastModificationTime = entity.LastModificationTime,
            LastModifierId = entity.LastModifierId,
            IsDeleted = entity.IsDeleted,
            DeleterId = entity.DeleterId,
            DeletionTime = entity.DeletionTime
        };
    }
}
