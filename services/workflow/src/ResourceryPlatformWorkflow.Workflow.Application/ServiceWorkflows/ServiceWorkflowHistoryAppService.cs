using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ResourceryPlatformWorkflow.Workflow.Permissions;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

[Authorize(WorkflowPermissions.ServiceWorkflowHistory.Default)]
public class ServiceWorkflowHistoryAppService(
    IRepository<ServiceWorkflowHistory, Guid> serviceWorkflowHistoryRepository
) : WorkflowAppService, IServiceWorkflowHistoryAppService
{
    private readonly IRepository<ServiceWorkflowHistory, Guid> _serviceWorkflowHistoryRepository =
        serviceWorkflowHistoryRepository;

    public async Task<ServiceWorkflowHistoryDto> GetAsync(Guid id)
    {
        var entity = await _serviceWorkflowHistoryRepository.GetAsync(id);
        return Map(entity);
    }

    public async Task<List<ServiceWorkflowHistoryDto>> GetListAsync()
    {
        var entities = await _serviceWorkflowHistoryRepository.GetListAsync();
        return entities.Select(Map).ToList();
    }

    [Authorize(WorkflowPermissions.ServiceWorkflowHistory.Create)]
    public async Task<ServiceWorkflowHistoryDto> CreateAsync(CreateUpdateServiceWorkflowHistoryDto input)
    {
        Check.NotNull(input, nameof(input));

        var entity = new ServiceWorkflowHistory(
            GuidGenerator.Create(),
            input.ServiceWorkflowInstanceId,
            input.Type,
            input.Action,
            input.Comment,
            input.PerformedByUserId,
            input.ServiceWorkflowStepId,
            input.ServiceWorkflowTaskId
        );

        entity = await _serviceWorkflowHistoryRepository.InsertAsync(entity, autoSave: true);
        return Map(entity);
    }

    [Authorize(WorkflowPermissions.ServiceWorkflowHistory.Update)]
    public async Task<ServiceWorkflowHistoryDto> UpdateAsync(
        Guid id,
        CreateUpdateServiceWorkflowHistoryDto input
    )
    {
        Check.NotNull(input, nameof(input));

        var entity = await _serviceWorkflowHistoryRepository.GetAsync(id);
        entity.SetAction(input.Action);
        entity.SetComment(input.Comment);

        entity = await _serviceWorkflowHistoryRepository.UpdateAsync(entity, autoSave: true);
        return Map(entity);
    }

    [Authorize(WorkflowPermissions.ServiceWorkflowHistory.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _serviceWorkflowHistoryRepository.DeleteAsync(id, autoSave: true);
    }

    private static ServiceWorkflowHistoryDto Map(ServiceWorkflowHistory entity)
    {
        return new ServiceWorkflowHistoryDto
        {
            Id = entity.Id,
            ServiceWorkflowInstanceId = entity.ServiceWorkflowInstanceId,
            ServiceWorkflowStepId = entity.ServiceWorkflowStepId,
            ServiceWorkflowTaskId = entity.ServiceWorkflowTaskId,
            Type = entity.Type,
            Action = entity.Action,
            Comment = entity.Comment,
            PerformedByUserId = entity.PerformedByUserId,
            PerformedAt = entity.PerformedAt
        };
    }
}
