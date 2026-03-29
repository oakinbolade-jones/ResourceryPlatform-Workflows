using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ResourceryPlatformWorkflow.Workflow.Permissions;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

[Authorize(WorkflowPermissions.ServiceWorkflowInstances.Default)]
public class ServiceWorkflowInstanceAppService(
    IRepository<ServiceWorkflowInstance, Guid> serviceWorkflowInstanceRepository
) : WorkflowAppService, IServiceWorkflowInstanceAppService
{
    private readonly IRepository<ServiceWorkflowInstance, Guid> _serviceWorkflowInstanceRepository =
        serviceWorkflowInstanceRepository;

    public async Task<ServiceWorkflowInstanceDto> GetAsync(Guid id)
    {
        var entity = await _serviceWorkflowInstanceRepository.GetAsync(id);
        return Map(entity);
    }

    public async Task<List<ServiceWorkflowInstanceDto>> GetListAsync()
    {
        var entities = await _serviceWorkflowInstanceRepository.GetListAsync();
        return entities.Select(Map).ToList();
    }

    [Authorize(WorkflowPermissions.ServiceWorkflowInstances.Create)]
    public async Task<ServiceWorkflowInstanceDto> CreateAsync(CreateUpdateServiceWorkflowInstanceDto input)
    {
        Check.NotNull(input, nameof(input));

        var entity = new ServiceWorkflowInstance(
            GuidGenerator.Create(),
            input.ServiceWorkflowId,
            input.RequestId
        );

        if (input.CurrentStepId.HasValue)
        {
            entity.StartStep(input.CurrentStepId.Value);
        }

        ApplyStatus(entity, input.Status);

        entity = await _serviceWorkflowInstanceRepository.InsertAsync(entity, autoSave: true);
        return Map(entity);
    }

    [Authorize(WorkflowPermissions.ServiceWorkflowInstances.Update)]
    public async Task<ServiceWorkflowInstanceDto> UpdateAsync(
        Guid id,
        CreateUpdateServiceWorkflowInstanceDto input
    )
    {
        Check.NotNull(input, nameof(input));

        var entity = await _serviceWorkflowInstanceRepository.GetAsync(id);

        if (input.CurrentStepId.HasValue)
        {
            entity.StartStep(input.CurrentStepId.Value);
        }

        ApplyStatus(entity, input.Status);

        entity = await _serviceWorkflowInstanceRepository.UpdateAsync(entity, autoSave: true);
        return Map(entity);
    }

    [Authorize(WorkflowPermissions.ServiceWorkflowInstances.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _serviceWorkflowInstanceRepository.DeleteAsync(id, autoSave: true);
    }

    private static void ApplyStatus(
        ServiceWorkflowInstance entity,
        ServiceWorkflowInstanceStatus targetStatus
    )
    {
        switch (targetStatus)
        {
            case ServiceWorkflowInstanceStatus.Pending:
            case ServiceWorkflowInstanceStatus.InProgress:
                break;
            case ServiceWorkflowInstanceStatus.Completed:
                entity.Complete();
                break;
            case ServiceWorkflowInstanceStatus.Rejected:
                entity.Reject();
                break;
            case ServiceWorkflowInstanceStatus.Cancelled:
                entity.Cancel();
                break;
            default:
                throw new BusinessException("Workflow:ServiceWorkflowInstances:InvalidStatus");
        }
    }

    private static ServiceWorkflowInstanceDto Map(ServiceWorkflowInstance entity)
    {
        return new ServiceWorkflowInstanceDto
        {
            Id = entity.Id,
            ServiceWorkflowId = entity.ServiceWorkflowId,
            RequestId = entity.RequestId,
            CurrentStepId = entity.CurrentStepId,
            Status = entity.Status,
            StartedAt = entity.StartedAt,
            CompletedAt = entity.CompletedAt,
            CreationTime = entity.CreationTime,
            CreatorId = entity.CreatorId,
            LastModificationTime = entity.LastModificationTime,
            LastModifierId = entity.LastModifierId,
            IsDeleted = entity.IsDeleted,
            DeleterId = entity.DeleterId,
            DeletionTime = entity.DeletionTime
        };
    }
}
