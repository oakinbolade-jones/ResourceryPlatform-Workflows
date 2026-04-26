using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using ResourceryPlatformWorkflow.Workflow.Services;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;

namespace ResourceryPlatformWorkflow.Workflow.SeedData;

public class ServicesDataSeedContributor(
    ICurrentTenant currentTenant,
    IRepository<Service, Guid> serviceRepository,
    IRepository<ServiceCenter, Guid> serviceCenterRepository
) : IDataSeedContributor, ITransientDependency
{
    private const string ServiceSeedFileName = "ServiceSeedData.json";

    private readonly ICurrentTenant _currentTenant = currentTenant;
    private readonly IRepository<Service, Guid> _serviceRepository = serviceRepository;
    private readonly IRepository<ServiceCenter, Guid> _serviceCenterRepository = serviceCenterRepository;

    public async Task SeedAsync(DataSeedContext context)
    {
        if (context.TenantId.HasValue)
        {
            return;
        }

        var serviceCentersExist = await _serviceCenterRepository.AnyAsync();
        if (!serviceCentersExist)
        {
            return;
        }

        var seedItems = await ReadSeedItemsAsync();
        if (seedItems.Count == 0)
        {
            return;
        }

        using (_currentTenant.Change(context.TenantId))
        {
            foreach (var item in seedItems)
            {
                if (string.IsNullOrWhiteSpace(item.Name) || string.IsNullOrWhiteSpace(item.ServiceCode))
                {
                    continue;
                }

                var exists = await _serviceRepository.AnyAsync(x => x.Code == item.ServiceCode);
                if (exists)
                {
                    continue;
                }

                var serviceCenter = await _serviceCenterRepository.FindAsync(
                    x => x.Code == item.ServiceCenterCode
                );

                if (serviceCenter == null)
                {
                    continue;
                }

                var description = string.IsNullOrWhiteSpace(item.Description)
                    ? item.Name
                    : item.Description;
                var displayName = string.IsNullOrWhiteSpace(item.DisplayName)
                    ? item.Name
                    : item.DisplayName;

                var service = new Service(
                    Guid.NewGuid(),
                    serviceCenter.Id,
                    item.Name,
                    item.ServiceCode,
                    displayName,
                    description
                );

                service.SetIsActive(item.IsActive);

                await _serviceRepository.InsertAsync(service, autoSave: true);
            }
        }
    }

    private static async Task<List<ServiceSeedItem>> ReadSeedItemsAsync()
    {
        await using var stream = OpenSeedStream();
        var seedItems = await JsonSerializer.DeserializeAsync<List<ServiceSeedItem>>(stream);
        return seedItems ?? [];
    }

    private static System.IO.Stream OpenSeedStream()
    {
        var assembly = typeof(ServicesDataSeedContributor).Assembly;
        var resourceName = Array.Find(
            assembly.GetManifestResourceNames(),
            name => name.EndsWith(ServiceSeedFileName, StringComparison.Ordinal)
        );

        if (resourceName != null)
        {
            return assembly.GetManifestResourceStream(resourceName)
                ?? throw new BusinessException("Workflow:SeedData:ServiceSeedFileNotFound");
        }

        throw new BusinessException("Workflow:SeedData:ServiceSeedFileNotFound");
    }

    private sealed class ServiceSeedItem
    {
        public string ServiceCode { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string ServiceCenterCode { get; set; }
    }
}
