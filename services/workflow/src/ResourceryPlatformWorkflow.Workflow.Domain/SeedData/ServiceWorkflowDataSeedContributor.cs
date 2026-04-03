using System;
using System.Collections.Generic;

using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ResourceryPlatformWorkflow.Workflow.Services;
using ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;

namespace ResourceryPlatformWorkflow.Workflow.SeedData;

public class ServiceWorkflowDataSeedContributor(
    ICurrentTenant currentTenant,
    IRepository<ServiceWorkflow, Guid> serviceWorkflowRepository
) : IDataSeedContributor, ITransientDependency
{
    private const string ServiceWorkflowSeedFileName = "ServiceWorkflowSeedData.json";

    private readonly ICurrentTenant _currentTenant = currentTenant;
    private readonly IRepository<ServiceWorkflow, Guid> _serviceWorkflowRepository =
        serviceWorkflowRepository;

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
                if (string.IsNullOrWhiteSpace(item.Name) || string.IsNullOrWhiteSpace(item.Code))
                {
                    continue;
                }

                var exists = await _serviceWorkflowRepository.AnyAsync(x => x.Code == item.Code);
                if (exists)
                {
                    continue;
                }

                var workflow = new ServiceWorkflow(
                    Guid.NewGuid(),
                    item.Name,
                    item.Code,
                    string.IsNullOrWhiteSpace(item.DisplayName) ? item.Name : item.DisplayName,
                    item.LeadTime,
                    item.LeadTimeType,
                    item.Name
                );
                workflow.SetIsActive(true);

                await _serviceWorkflowRepository.InsertAsync(workflow, autoSave: true);
            }
        }
    }

    private static async Task<List<ServiceWorkflowSeedItem>> ReadSeedItemsAsync()
    {
        await using var stream = OpenSeedStream();
        var seedItems = await JsonSerializer.DeserializeAsync<List<ServiceWorkflowSeedItem>>(stream);
        return seedItems ?? [];
    }

    private static System.IO.Stream OpenSeedStream()
    {
        var assembly = typeof(ServiceWorkflowDataSeedContributor).Assembly;
        var resourceName = Array.Find(
            assembly.GetManifestResourceNames(),
            name => name.EndsWith(ServiceWorkflowSeedFileName, StringComparison.Ordinal)
        );

        if (resourceName != null)
        {
            return assembly.GetManifestResourceStream(resourceName)
                ?? throw new BusinessException("Workflow:SeedData:ServiceWorkflowSeedFileNotFound");
        }

        throw new BusinessException("Workflow:SeedData:ServiceWorkflowSeedFileNotFound");
    }

    private sealed class ServiceWorkflowSeedItem
    {
        public string Code { get; set; }
        public string DisplayName { get; set; }
        public string LeadTime { get; set; }
        public string LeadTimeType { get; set; }
        public string Name { get; set; }
    }
}
