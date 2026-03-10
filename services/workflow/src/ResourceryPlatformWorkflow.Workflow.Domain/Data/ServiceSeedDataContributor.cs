using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ResourceryWorkflow.Workflow.Departments;
using ResourceryWorkflow.Workflow.Services;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;

namespace ResourceryWorkflow.Workflow.Data;

public class ServiceSeedDataContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Service, Guid> _serviceRepository;
    private readonly IRepository<Department, Guid> _departmentRepository;
    private readonly ICurrentTenant _currentTenant;

    public ServiceSeedDataContributor(
        IRepository<Service, Guid> serviceRepository,
        IRepository<Department, Guid> departmentRepository,
        ICurrentTenant currentTenant)
    {
        _serviceRepository = serviceRepository;
        _departmentRepository = departmentRepository;
        _currentTenant = currentTenant;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        using (_currentTenant.Change(context?.TenantId))
        {
            var services = await LoadServicesFromJsonAsync();
            if (services.Count == 0)
            {
                return;
            }

            var existingServiceCodes = (await _serviceRepository.GetListAsync())
                .Select(item => item.Code)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var service in services)
            {
                if (existingServiceCodes.Contains(service.Code))
                {
                    continue;
                }

                await _serviceRepository.InsertAsync(service, autoSave: true);
            }
        }
    }

    private async Task<List<Service>> LoadServicesFromJsonAsync()
    {
        var jsonPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Data",
            "json",
            "ServiceSeedData.json"
        );

        if (!File.Exists(jsonPath))
        {
            return new List<Service>();
        }

        var json = await File.ReadAllTextAsync(jsonPath);

        var seedItems = JsonSerializer.Deserialize<List<ServiceSeedItem>>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<ServiceSeedItem>();

        var departmentsByCode = (await _departmentRepository.GetListAsync())
            .Where(item => !string.IsNullOrWhiteSpace(item.Code))
            .GroupBy(item => item.Code, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First().Id, StringComparer.OrdinalIgnoreCase);

        var services = new List<Service>();

        foreach (var seedItem in seedItems)
        {
            if (string.IsNullOrWhiteSpace(seedItem.DeptCode)
                || string.IsNullOrWhiteSpace(seedItem.Name)
                || string.IsNullOrWhiteSpace(seedItem.Code))
            {
                continue;
            }

            if (!departmentsByCode.TryGetValue(seedItem.DeptCode, out var departmentId))
            {
                continue;
            }

            var service = new Service(
                Guid.NewGuid(),
                departmentId,
                seedItem.Name,
                seedItem.Code,
                null
            );

            services.Add(service);
        }

        return services;
    }

    private sealed class ServiceSeedItem
    {
        public string DeptCode { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }
    }
}
