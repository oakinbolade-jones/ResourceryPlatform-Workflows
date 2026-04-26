using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace ResourceryPlatformWorkflow.Workflow.Services;

public class ServiceCenterManager : DomainService
{
    private readonly IRepository<ServiceCenter, Guid> _serviceCenterRepository;
    private readonly IRepository<Service, Guid> _serviceRepository;

    public ServiceCenterManager(
        IRepository<ServiceCenter, Guid> serviceCenterRepository,
        IRepository<Service, Guid> serviceRepository
    )
    {
        _serviceCenterRepository = serviceCenterRepository;
        _serviceRepository = serviceRepository;
    }

    public async Task<ServiceCenter> CreateAsync(
        Guid id,
        string name,
        string displayName,
        string description,
        string code
    )
    {
        await EnsureCodeIsUniqueAsync(code);

        var entity = new ServiceCenter(id, name, displayName, description, code);
        return await _serviceCenterRepository.InsertAsync(entity, autoSave: true);
    }

    public async Task<ServiceCenter> UpdateAsync(
        Guid id,
        string name,
        string displayName,
        string description,
        string code
    )
    {
        var entity = await _serviceCenterRepository.GetAsync(id);

        await EnsureCodeIsUniqueAsync(code, id);

        entity.SetName(name);
        entity.SetDisplayName(displayName);
        entity.SetDescription(description);
        entity.SetCode(code);

        return await _serviceCenterRepository.UpdateAsync(entity, autoSave: true);
    }

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
}
