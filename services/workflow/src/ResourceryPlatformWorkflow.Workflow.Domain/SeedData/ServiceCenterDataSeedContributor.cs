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

public class ServiceCenterDataSeedContributor(
    ICurrentTenant currentTenant,
    IRepository<ServiceCenter, Guid> serviceCenterRepository
) : IDataSeedContributor, ITransientDependency
{
    private const string ServiceCenterSeedFileName = "ServiceCenterSeedData.json";

    private readonly ICurrentTenant _currentTenant = currentTenant;
    private readonly IRepository<ServiceCenter, Guid> _serviceCenterRepository = serviceCenterRepository;

    public async Task SeedAsync(DataSeedContext context)
    {
        if (context.TenantId.HasValue)
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
                if (string.IsNullOrWhiteSpace(item.Code))
                {
                    continue;
                }

                var exists = await _serviceCenterRepository.AnyAsync(x => x.Code == item.Code);
                if (exists)
                {
                    continue;
                }

                var serviceCenter = new ServiceCenter(
                    Guid.NewGuid(),
                    item.Name,
                    item.DisplayName,
                    item.Description,
                    item.Code
                );

                await _serviceCenterRepository.InsertAsync(serviceCenter, autoSave: true);
            }
        }
    }

    private static async Task<List<ServiceCenterSeedItem>> ReadSeedItemsAsync()
    {
        await using var stream = OpenSeedStream();
        var seedItems = await JsonSerializer.DeserializeAsync<List<ServiceCenterSeedItem>>(stream);
        return seedItems ?? [];
    }

    private static System.IO.Stream OpenSeedStream()
    {
        var assembly = typeof(ServiceCenterDataSeedContributor).Assembly;
        var resourceName = Array.Find(
            assembly.GetManifestResourceNames(),
            name => name.EndsWith(ServiceCenterSeedFileName, StringComparison.Ordinal)
        );

        if (resourceName != null)
        {
            return assembly.GetManifestResourceStream(resourceName)
                ?? throw new BusinessException("Workflow:SeedData:ServiceCenterSeedFileNotFound");
        }

        throw new BusinessException("Workflow:SeedData:ServiceCenterSeedFileNotFound");
    }

    private sealed class ServiceCenterSeedItem
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
    }
}