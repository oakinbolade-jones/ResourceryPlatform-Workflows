using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ResourceryWorkflow.Workflow.Departments;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;

namespace ResourceryWorkflow.Workflow.Data;

public class DepartmentSeedDataContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Department, Guid> _departmentRepository;
    private readonly ICurrentTenant _currentTenant;

    public DepartmentSeedDataContributor(
        IRepository<Department, Guid> departmentRepository,
        ICurrentTenant currentTenant)
    {
        _departmentRepository = departmentRepository;
        _currentTenant = currentTenant;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        using (_currentTenant.Change(context?.TenantId))
        {
            // Check if departments already exist
            var existingCount = await _departmentRepository.GetCountAsync();
            if (existingCount > 0)
            {
                return; // Data already seeded
            }

            // Load departments from JSON
            var departments = await LoadDepartmentsFromJsonAsync();

            // Insert departments
            foreach (var department in departments)
            {
                await _departmentRepository.InsertAsync(department, autoSave: true);
            }
        }
    }

    private async Task<List<Department>> LoadDepartmentsFromJsonAsync()
    {
        var jsonPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Data",
            "json",
            "Department.json"
        );

        if (!File.Exists(jsonPath))
        {
            return new List<Department>();
        }

        var json = await File.ReadAllTextAsync(jsonPath);
        
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        var departments = new List<Department>();

        foreach (var element in root.EnumerateArray())
        {
            var name = element.GetProperty("Name").GetString();
            var description = element.GetProperty("Description").GetString();
            var code = element.GetProperty("Code").GetString();

            var department = new Department(
                Guid.NewGuid(),
                name,
                code,
                description
            );

            departments.Add(department);
        }

        return departments;
    }
}
