using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;

namespace ResourceryPlatformWorkflow.Workflow.SeedData;

public class ServiceWorkflowStepDataSeedContributor(
    ICurrentTenant currentTenant,
    IRepository<ServiceWorkflow, Guid> serviceWorkflowRepository,
    IRepository<ServiceWorkflowStep, Guid> serviceWorkflowStepRepository
) : IDataSeedContributor, ITransientDependency
{
    private const string ServiceWorkflowStepSeedFileName = "ServiceWorkflowStepSeedData.json";

    private readonly ICurrentTenant _currentTenant = currentTenant;
    private readonly IRepository<ServiceWorkflow, Guid> _serviceWorkflowRepository = serviceWorkflowRepository;
    private readonly IRepository<ServiceWorkflowStep, Guid> _serviceWorkflowStepRepository = serviceWorkflowStepRepository;

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
                if (string.IsNullOrWhiteSpace(item.ServiceWorkflowCode) || 
                    string.IsNullOrWhiteSpace(item.ServiceWorkflowStepCode))
                {
                    continue;
                }

                var serviceWorkflow = await _serviceWorkflowRepository.FindAsync(
                    x => x.Code == item.ServiceWorkflowCode
                );

                if (serviceWorkflow == null)
                {
                    continue;
                }

                var exists = await _serviceWorkflowStepRepository.AnyAsync(
                    x => x.ServiceWorkflowId == serviceWorkflow.Id && x.Code == item.Code
                );

                if (exists)
                {
                    continue;
                }

                var name = item.Name;
                var code = item.Code;
                var description = string.IsNullOrWhiteSpace(item.Name)
                    ? code
                    : item.Name;
                var displayName = string.IsNullOrWhiteSpace(item.DisplayName)
                    ? name
                    : item.DisplayName;
                var displayNameOutput = string.IsNullOrWhiteSpace(item.DisplayNameOutput)
                    ? name
                    : item.DisplayNameOutput;
                var tatType = string.IsNullOrWhiteSpace(item.TATType)
                    ? "Value"
                    : item.TATType;
                var tatUnit = string.IsNullOrWhiteSpace(item.TATUnit)
                    ? "Minutes"
                    : item.TATUnit;

                if (!int.TryParse(item.Order, out var order))
                {
                    order = 1;
                }

                var step = new ServiceWorkflowStep(
                    Guid.NewGuid(),
                    serviceWorkflow.Id,
                    name,
                    code,
                    description,
                    displayName,
                    displayNameOutput,
                    item.Output ?? string.Empty,
                    tatType,
                    tatUnit,
                    order
                );

                await _serviceWorkflowStepRepository.InsertAsync(step, autoSave: true);
            }
        }
    }

    private static async Task<List<ServiceWorkflowStepSeedItem>> ReadSeedItemsAsync()
    {
        await using var stream = OpenSeedStream();
        var seedItems = await JsonSerializer.DeserializeAsync<List<ServiceWorkflowStepSeedItem>>(stream);
        return seedItems ?? [];
    }

    private static System.IO.Stream OpenSeedStream()
    {
        var assembly = typeof(ServiceWorkflowStepDataSeedContributor).Assembly;
        var resourceName = Array.Find(
            assembly.GetManifestResourceNames(),
            name => name.EndsWith(ServiceWorkflowStepSeedFileName, StringComparison.Ordinal)
        );

        if (resourceName != null)
        {
            return assembly.GetManifestResourceStream(resourceName)
                ?? throw new BusinessException("Workflow:SeedData:ServiceWorkflowStepSeedFileNotFound");
        }

        throw new BusinessException("Workflow:SeedData:ServiceWorkflowStepSeedFileNotFound");
    }

    private sealed class ServiceWorkflowStepSeedItem
    {
        public string ServiceWorkflowCode { get; set; }
        public string ServiceWorkflowStepCode { get; set; }
        public string Order { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string DisplayNameOutput { get; set; }
        public string Output { get; set; }
        public string Actors { get; set; }
        public string TATType { get; set; }
        public string TATUnit { get; set; }
        public string Code { get; set; }
    }
}
