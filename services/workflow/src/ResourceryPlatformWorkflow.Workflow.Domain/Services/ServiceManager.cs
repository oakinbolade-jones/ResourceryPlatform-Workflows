using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace ResourceryPlatformWorkflow.Workflow.Services;

public class ServiceManager : DomainService
{
    private readonly IRepository<Service, Guid> _serviceRepository;
    private readonly IRepository<ServiceCenter, Guid> _serviceCenterRepository;

    public ServiceManager(
        IRepository<Service, Guid> serviceRepository,
        IRepository<ServiceCenter, Guid> serviceCenterRepository
    )
    {
        _serviceRepository = serviceRepository;
        _serviceCenterRepository = serviceCenterRepository;
    }

    public async Task<Service> CreateAsync(
        Guid id,
        Guid serviceCenterId,
        string name,
        string code,
        string displayName,
        string description,
        bool isActive
    )
    {
        await EnsureServiceCenterExistsAsync(serviceCenterId);

        var entity = new Service(id, serviceCenterId, name, code, displayName, description);
        entity.SetIsActive(isActive);

        return await _serviceRepository.InsertAsync(entity, autoSave: true);
    }

    public async Task<Service> UpdateAsync(
        Guid id,
        Guid serviceCenterId,
        string name,
        string code,
        string displayName,
        string description,
        bool isActive
    )
    {
        await EnsureServiceCenterExistsAsync(serviceCenterId);

        var entity = await _serviceRepository.GetAsync(id);
        entity.SetServiceCenter(serviceCenterId);
        entity.SetName(name);
        entity.SetCode(code);
        entity.SetDisplayName(displayName);
        entity.SetDescription(description);
        entity.SetIsActive(isActive);

        return await _serviceRepository.UpdateAsync(entity, autoSave: true);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _serviceRepository.DeleteAsync(id, autoSave: true);
    }

    private async Task EnsureServiceCenterExistsAsync(Guid serviceCenterId)
    {
        var exists = await _serviceCenterRepository.AnyAsync(x => x.Id == serviceCenterId);
        if (!exists)
        {
            throw new BusinessException("Workflow:Services:ServiceCenterNotFound");
        }
    }
}
