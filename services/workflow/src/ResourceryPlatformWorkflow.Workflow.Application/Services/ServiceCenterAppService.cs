using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ResourceryPlatformWorkflow.Workflow.Permissions;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace ResourceryPlatformWorkflow.Workflow.Services;

[Authorize(WorkflowPermissions.ServiceCenters.Default)]
public class ServiceCenterAppService(
    IRepository<ServiceCenter, Guid> serviceCenterRepository,
    ServiceCenterManager serviceCenterManager
) : WorkflowAppService, IServiceCenterAppService
{
    private readonly IRepository<ServiceCenter, Guid> _serviceCenterRepository = serviceCenterRepository;
    private readonly ServiceCenterManager _serviceCenterManager = serviceCenterManager;

    public async Task<ServiceCenterDto> GetAsync(Guid id)
    {
        var entity = await _serviceCenterRepository.GetAsync(id);
        return Map(entity);
    }

    public async Task<List<ServiceCenterDto>> GetListAsync()
    {
        var entities = await _serviceCenterRepository.GetListAsync();
        return entities.Select(Map).ToList();
    }

    [Authorize(WorkflowPermissions.ServiceCenters.Create)]
    public async Task<ServiceCenterDto> CreateAsync(CreateUpdateServiceCenterDto input)
    {
        Check.NotNull(input, nameof(input));

        var entity = await _serviceCenterManager.CreateAsync(
            GuidGenerator.Create(),
            input.Name,
            input.DisplayName,
            input.Description,
            input.Code
        );

        return Map(entity);
    }

    [Authorize(WorkflowPermissions.ServiceCenters.Update)]
    public async Task<ServiceCenterDto> UpdateAsync(Guid id, CreateUpdateServiceCenterDto input)
    {
        Check.NotNull(input, nameof(input));

        var entity = await _serviceCenterManager.UpdateAsync(
            id,
            input.Name,
            input.DisplayName,
            input.Description,
            input.Code
        );

        return Map(entity);
    }

    [Authorize(WorkflowPermissions.ServiceCenters.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _serviceCenterManager.DeleteAsync(id);
    }

    private static ServiceCenterDto Map(ServiceCenter entity)
    {
        return new ServiceCenterDto
        {
            Id = entity.Id,
            Name = entity.Name,
            DisplayName = entity.DisplayName,
            Description = entity.Description,
            Code = entity.Code,
            CreationTime = entity.CreationTime,
            CreatorId = entity.CreatorId,
            LastModificationTime = entity.LastModificationTime,
            LastModifierId = entity.LastModifierId,
            IsDeleted = entity.IsDeleted,
            DeleterId = entity.DeleterId,
            DeletionTime = entity.DeletionTime,
        };
    }
}