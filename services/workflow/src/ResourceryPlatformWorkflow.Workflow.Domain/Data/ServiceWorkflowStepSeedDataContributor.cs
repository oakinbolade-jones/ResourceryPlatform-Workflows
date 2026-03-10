using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ResourceryWorkflow.Workflow.ServiceWorkflows;
using ResourceryWorkflow.Workflow.Services;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;

namespace ResourceryWorkflow.Workflow.Data;

public class ServiceWorkflowStepSeedDataContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<ServiceWorkflowStep, Guid> _stepRepository;
    private readonly IRepository<ServiceWorkflow, Guid> _workflowRepository;
    private readonly IRepository<Service, Guid> _serviceRepository;
    private readonly ICurrentTenant _currentTenant;

    public ServiceWorkflowStepSeedDataContributor(
        IRepository<ServiceWorkflowStep, Guid> stepRepository,
        IRepository<ServiceWorkflow, Guid> workflowRepository,
        IRepository<Service, Guid> serviceRepository,
        ICurrentTenant currentTenant)
    {
        _stepRepository = stepRepository;
        _workflowRepository = workflowRepository;
        _serviceRepository = serviceRepository;
        _currentTenant = currentTenant;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        using (_currentTenant.Change(context?.TenantId))
        {
            var steps = await LoadStepsFromJsonAsync();
            if (steps.Count == 0)
            {
                return;
            }

            var existing = await _stepRepository.GetListAsync();

            var existingKeys = existing
                .Select(item => new StepKey(item.ServiceWorkflowId, item.Order, item.Title))
                .ToHashSet();

            foreach (var step in steps)
            {
                var key = new StepKey(step.ServiceWorkflowId, step.Order, step.Title);
                if (existingKeys.Contains(key))
                {
                    continue;
                }

                await _stepRepository.InsertAsync(step, autoSave: true);
            }
        }
    }

    private async Task<List<ServiceWorkflowStep>> LoadStepsFromJsonAsync()
    {
        var jsonPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Data",
            "json",
            "ServiceWorkflowStepSeedData.json"
        );

        if (!File.Exists(jsonPath))
        {
            return new List<ServiceWorkflowStep>();
        }

        var json = await File.ReadAllTextAsync(jsonPath);

        var seedItems = JsonSerializer.Deserialize<List<ServiceWorkflowStepSeedItem>>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<ServiceWorkflowStepSeedItem>();

        if (seedItems.Count == 0)
        {
            return new List<ServiceWorkflowStep>();
        }

        // Get ServiceId by Service.Code (using ServiceCode from JSON)
        var servicesByCode = (await _serviceRepository.GetListAsync())
            .Where(item => !string.IsNullOrWhiteSpace(item.Code))
            .GroupBy(item => item.Code, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First().Id, StringComparer.OrdinalIgnoreCase);

        // Map ServiceId -> ServiceWorkflow (assumes a single workflow per service)
        var workflows = await _workflowRepository.GetListAsync();
        var workflowByServiceId = workflows
            .GroupBy(wf => wf.ServiceId)
            .ToDictionary(g => g.Key, g => g.First().Id);

        var steps = new List<ServiceWorkflowStep>();

        foreach (var seedItem in seedItems)
        {
            if (string.IsNullOrWhiteSpace(seedItem.ServiceCode)
                || string.IsNullOrWhiteSpace(seedItem.Step)
                || string.IsNullOrWhiteSpace(seedItem.Description))
            {
                continue;
            }

            if (!servicesByCode.TryGetValue(seedItem.ServiceCode, out var serviceId))
            {
                continue;
            }

            if (!workflowByServiceId.TryGetValue(serviceId, out var workflowId))
            {
                continue;
            }

            if (!int.TryParse(seedItem.Step, out var order))
            {
                continue;
            }

            var title = $"Step {order}";

            var step = new ServiceWorkflowStep(
                Guid.NewGuid(),
                workflowId,
                title,
                order,
                seedItem.Description
            );

            steps.Add(step);
        }

        return steps;
    }

    private readonly record struct StepKey(Guid ServiceWorkflowId, int Order, string Title);

    private sealed class ServiceWorkflowStepSeedItem
    {
        public string Step { get; set; }

        public string Description { get; set; }

        public string Actor { get; set; }

        public string ServiceCode { get; set; }
    }
}
