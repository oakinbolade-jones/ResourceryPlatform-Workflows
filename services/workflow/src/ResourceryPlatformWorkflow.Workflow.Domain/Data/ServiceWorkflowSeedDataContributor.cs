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

public class ServiceWorkflowSeedDataContributor : IDataSeedContributor, ITransientDependency
{
	private readonly IRepository<ServiceWorkflow, Guid> _serviceWorkflowRepository;
	private readonly IRepository<Service, Guid> _serviceRepository;
	private readonly ICurrentTenant _currentTenant;

	public ServiceWorkflowSeedDataContributor(
		IRepository<ServiceWorkflow, Guid> serviceWorkflowRepository,
		IRepository<Service, Guid> serviceRepository,
		ICurrentTenant currentTenant)
	{
		_serviceWorkflowRepository = serviceWorkflowRepository;
		_serviceRepository = serviceRepository;
		_currentTenant = currentTenant;
	}

	public async Task SeedAsync(DataSeedContext context)
	{
		using (_currentTenant.Change(context?.TenantId))
		{
			var workflows = await LoadWorkflowsFromJsonAsync();
			if (workflows.Count == 0)
			{
				return;
			}

			var existing = await _serviceWorkflowRepository.GetListAsync();

			var existingKeys = existing
				.Select(item => new WorkflowKey(item.ServiceId, item.Title))
				.ToHashSet();

			foreach (var workflow in workflows)
			{
				var key = new WorkflowKey(workflow.ServiceId, workflow.Title);
				if (existingKeys.Contains(key))
				{
					continue;
				}

				await _serviceWorkflowRepository.InsertAsync(workflow, autoSave: true);
			}
		}
	}

	private async Task<List<ServiceWorkflow>> LoadWorkflowsFromJsonAsync()
	{
		var jsonPath = Path.Combine(
			AppDomain.CurrentDomain.BaseDirectory,
			"Data",
			"json",
			"ServiceWorkflowSeedData.json"
		);

		if (!File.Exists(jsonPath))
		{
			return new List<ServiceWorkflow>();
		}

		var json = await File.ReadAllTextAsync(jsonPath);

		var seedItems = JsonSerializer.Deserialize<List<ServiceWorkflowSeedItem>>(
			json,
			new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			}) ?? new List<ServiceWorkflowSeedItem>();

		if (seedItems.Count == 0)
		{
			return new List<ServiceWorkflow>();
		}

		var servicesByCode = (await _serviceRepository.GetListAsync())
			.Where(item => !string.IsNullOrWhiteSpace(item.Code))
			.GroupBy(item => item.Code, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(group => group.Key, group => group.First().Id, StringComparer.OrdinalIgnoreCase);

		var workflows = new List<ServiceWorkflow>();

		foreach (var seedItem in seedItems)
		{
			if (string.IsNullOrWhiteSpace(seedItem.ServiceCode)
				|| string.IsNullOrWhiteSpace(seedItem.Name))
			{
				continue;
			}

			if (!servicesByCode.TryGetValue(seedItem.ServiceCode, out var serviceId))
			{
				continue;
			}

			var workflow = new ServiceWorkflow(
				Guid.NewGuid(),
				serviceId,
				seedItem.Name,
				null,
				seedItem.Actvities,
				seedItem.Outcome,
				seedItem.Details
			);

			workflows.Add(workflow);
		}

		return workflows;
	}

	private readonly record struct WorkflowKey(Guid ServiceId, string Title);

	private sealed class ServiceWorkflowSeedItem
	{
		public string DeptCode { get; set; }

		public string Name { get; set; }

		public string ServiceCode { get; set; }

		public string Actvities { get; set; }

		public string Outcome { get; set; }

		public string Details { get; set; }
	}
}

