using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ResourceryPlatformWorkflow.Workflow.Permissions;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

[Authorize(WorkflowPermissions.ServiceWorkflowTasks.Default)]
public class ServiceWorkflowTaskAppService(IRepository<ServiceWorkflowTask, Guid> serviceWorkflowTaskRepository)
    : WorkflowAppService,
        IServiceWorkflowTaskAppService
{
    private readonly IRepository<ServiceWorkflowTask, Guid> _serviceWorkflowTaskRepository =
        serviceWorkflowTaskRepository;

    public async Task<ServiceWorkflowTaskDto> GetAsync(Guid id)
    {
        var entity = await _serviceWorkflowTaskRepository.GetAsync(id);
        return Map(entity);
    }

    public async Task<List<ServiceWorkflowTaskDto>> GetListAsync()
    {
        var entities = await _serviceWorkflowTaskRepository.GetListAsync();
        return entities.Select(Map).ToList();
    }

    [Authorize(WorkflowPermissions.ServiceWorkflowTasks.Create)]
    public async Task<ServiceWorkflowTaskDto> CreateAsync(CreateUpdateServiceWorkflowTaskDto input)
    {
        Check.NotNull(input, nameof(input));

        var entity = new ServiceWorkflowTask(
            GuidGenerator.Create(),
            input.ServiceWorkflowInstanceId,
            input.ServiceWorkflowStepId,
            input.Title,
            input.Description,
            input.AssigneeUserId,
            input.DueDate
        );

        ApplyStatus(entity, input.Status);

        entity = await _serviceWorkflowTaskRepository.InsertAsync(entity, autoSave: true);
        return Map(entity);
    }

    [Authorize(WorkflowPermissions.ServiceWorkflowTasks.Update)]
    public async Task<ServiceWorkflowTaskDto> UpdateAsync(Guid id, CreateUpdateServiceWorkflowTaskDto input)
    {
        Check.NotNull(input, nameof(input));

        var entity = await _serviceWorkflowTaskRepository.GetAsync(id);
        entity.SetTitle(input.Title);
        entity.SetDescription(input.Description);
        entity.SetAssigneeUserId(input.AssigneeUserId);
        entity.SetDueDate(input.DueDate);

        ApplyStatus(entity, input.Status);

        entity = await _serviceWorkflowTaskRepository.UpdateAsync(entity, autoSave: true);
        return Map(entity);
    }

    [Authorize(WorkflowPermissions.ServiceWorkflowTasks.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _serviceWorkflowTaskRepository.DeleteAsync(id, autoSave: true);
    }

    private static void ApplyStatus(ServiceWorkflowTask entity, ServiceWorkflowTaskStatus targetStatus)
    {
        switch (targetStatus)
        {
            case ServiceWorkflowTaskStatus.Pending:
                break;
            case ServiceWorkflowTaskStatus.InProgress:
                entity.Start();
                break;
            case ServiceWorkflowTaskStatus.Completed:
                entity.Complete();
                break;
            case ServiceWorkflowTaskStatus.Rejected:
                entity.Reject();
                break;
            case ServiceWorkflowTaskStatus.Cancelled:
                entity.Cancel();
                break;
            default:
                throw new BusinessException("Workflow:ServiceWorkflowTasks:InvalidStatus");
        }
    }

    private static ServiceWorkflowTaskDto Map(ServiceWorkflowTask entity)
    {
        return new ServiceWorkflowTaskDto
        {
            Id = entity.Id,
            ServiceWorkflowInstanceId = entity.ServiceWorkflowInstanceId,
            ServiceWorkflowStepId = entity.ServiceWorkflowStepId,
            Title = entity.Title,
            Description = entity.Description,
            AssigneeUserId = entity.AssigneeUserId,
            Status = entity.Status,
            DueDate = entity.DueDate,
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
