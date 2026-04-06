using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using ResourceryPlatformWorkflow.Workflow.Meetings;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;

namespace ResourceryPlatformWorkflow.Workflow.SeedData;

public class MeetingRequirementDataSeedContributor(ICurrentTenant currentTenant, IRepository<MeetingRequirement, Guid> meetingRequirementRepository) : IDataSeedContributor, ITransientDependency
{

    private const string MeetingRequirementSeedFileName = "MeetingRequirementSeedData.json";

    private readonly ICurrentTenant _currentTenant = currentTenant;
    private readonly IRepository<MeetingRequirement, Guid> _meetingRequirementRepository = meetingRequirementRepository;

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
                if (string.IsNullOrWhiteSpace(item.ItemCode))
                {
                    continue;
                }

                var exists = await _meetingRequirementRepository.AnyAsync(x => x.ItemCode == item.ItemCode);
                if (exists)
                {
                    continue;
                }

                var meetingRequirement = new MeetingRequirement(
                    Guid.NewGuid(),
                    item.ItemCode,
                    item.ItemName,
                    item.ItemCategory,
                    item.ServiceCenterCode,
                    item.DisplayNameItemName,
                    item.DisplayNameServiceCenter,
                    item.DisplayNameItemCategory
                );
                await _meetingRequirementRepository.InsertAsync(meetingRequirement, autoSave: true);
            }
        }
    }


    private static async Task<List<MeetingRequirementSeedItem>> ReadSeedItemsAsync()
    {
        await using var stream = OpenSeedStream();
        var seedItems = await JsonSerializer.DeserializeAsync<List<MeetingRequirementSeedItem>>(stream);
       return seedItems ?? new List<MeetingRequirementSeedItem>();
    }

    private static System.IO.Stream OpenSeedStream()
    {
        var assembly = typeof(MeetingRequirementDataSeedContributor).Assembly;
        var resourceName = Array.Find(
            assembly.GetManifestResourceNames(),
            name => name.EndsWith(MeetingRequirementSeedFileName, StringComparison.Ordinal)
        );

        if (resourceName != null)
        {
            return assembly.GetManifestResourceStream(resourceName)
                ?? throw new BusinessException("SeedData:MeetingRequirementSeedFileNotFound");
        }

        throw new BusinessException("SeedData:MeetingRequirementSeedFileNotFound");
    }

    private sealed class MeetingRequirementSeedItem
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemCategory { get; set; }
        public string ServiceCenterCode { get; set; }

        public string DisplayNameItemName { get; set; }
        public string DisplayNameServiceCenter { get; set; }
        public string DisplayNameItemCategory { get; set; }
    }
}

