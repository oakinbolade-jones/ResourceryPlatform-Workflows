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
    IRepository<Service, Guid> serviceRepository
) : WorkflowAppService, IServiceCenterAppService
{
    private readonly IRepository<ServiceCenter, Guid> _serviceCenterRepository = serviceCenterRepository;
    private readonly IRepository<Service, Guid> _serviceRepository = serviceRepository;

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
        await EnsureCodeIsUniqueAsync(input.Code);

        var entity = new ServiceCenter(
            GuidGenerator.Create(),
            input.Name,
            input.DisplayName,
            input.Description,
            input.Code
        );

        entity = await _serviceCenterRepository.InsertAsync(entity, autoSave: true);
        return Map(entity);
    }

    [Authorize(WorkflowPermissions.ServiceCenters.Update)]
    public async Task<ServiceCenterDto> UpdateAsync(Guid id, CreateUpdateServiceCenterDto input)
    {
        Check.NotNull(input, nameof(input));
        await EnsureCodeIsUniqueAsync(input.Code, id);

        var entity = await _serviceCenterRepository.GetAsync(id);
        entity.SetName(input.Name);
        entity.SetDisplayName(input.DisplayName);
        entity.SetDescription(input.Description);
        entity.SetCode(input.Code);

        entity = await _serviceCenterRepository.UpdateAsync(entity, autoSave: true);
        return Map(entity);
    }

    [Authorize(WorkflowPermissions.ServiceCenters.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        var isInUse = await _serviceRepository.AnyAsync(x => x.ServiceCenterId == id);
        if (isInUse)
        {
            throw new BusinessException("Workflow:ServiceCenters:InUse");
        }

        await _serviceCenterRepository.DeleteAsync(id, autoSave: true);
    }

    private async Task EnsureCodeIsUniqueAsync(string code, Guid? excludingId = null)
    {
        var normalizedCode = code?.Trim();
        var exists = await _serviceCenterRepository.AnyAsync(x =>
            x.Code == normalizedCode && (!excludingId.HasValue || x.Id != excludingId.Value)
        );

        if (exists)
        {
            throw new BusinessException("Workflow:ServiceCenters:CodeAlreadyExists");
        }
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