using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ResourceryPlatformWorkflow.Workflow.Meetings;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace ResourceryPlatformWorkflow.Workflow.Domain.SeedData;

public class MeetingRequirementSeedDto
{
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string ItemCategory { get; set; }
    public string ServiceCenterCode { get; set; }

    public string DisplayNameItemName { get; set; }
    public string DisplayNameServiceCenter { get; set; }
    public string DisplayNameItemCategory { get; set; }
}

public class MeetingRequirementDataSeedContributor : IDataSeedContributor
{
    private readonly IRepository<MeetingRequirement, Guid> _meetingRequirementRepository;

    public MeetingRequirementDataSeedContributor(IRepository<MeetingRequirement, Guid> meetingRequirementRepository)
    {
        _meetingRequirementRepository = meetingRequirementRepository;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "SeedData", "Json", "MeetingRequirementItemsSeedData.json");

        if (!File.Exists(jsonFilePath))
        {
            return;
        }

        var json = await File.ReadAllTextAsync(jsonFilePath);
        var seedData = JsonSerializer.Deserialize<List<MeetingRequirementSeedDto>>(json);

        foreach (var item in seedData)
        {
            var existing = await _meetingRequirementRepository.FirstOrDefaultAsync(x => x.ItemCode == item.ItemCode);
            if (existing == null)
            {
                await _meetingRequirementRepository.InsertAsync(new MeetingRequirement
                {
                    ItemCode = item.ItemCode,
                    ItemName = item.ItemName,
                    Category = item.ItemCategory,
                    ServiceCenterCode = item.ServiceCenterCode,
                    DisplayNameItemName = item.DisplayNameItemName,
                    DisplayNameServiceCenter = item.DisplayNameServiceCenter,
                    DisplayNameItemCategory = item.DisplayNameItemCategory
                });
            }
        }
    }
}