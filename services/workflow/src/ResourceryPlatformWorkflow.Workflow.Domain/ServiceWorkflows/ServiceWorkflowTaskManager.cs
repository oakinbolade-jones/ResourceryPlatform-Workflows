using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

public class ServiceWorkflowTaskManager : DomainService
{
    private readonly IRepository<ServiceWorkflowTask, Guid> _serviceWorkflowTaskRepository;

    public ServiceWorkflowTaskManager(IRepository<ServiceWorkflowTask, Guid> serviceWorkflowTaskRepository)
    {
        _serviceWorkflowTaskRepository = serviceWorkflowTaskRepository;
    }

    public async Task<ServiceWorkflowTask> CreateAsync(
        Guid id,
        Guid serviceWorkflowInstanceId,
        Guid serviceWorkflowStepId,
        string title,
        string description,
        Guid? assigneeUserId,
        DateTime? dueDate,
        ServiceWorkflowTaskStatus status
    )
    {
        var entity = new ServiceWorkflowTask(
            id,
            serviceWorkflowInstanceId,
            serviceWorkflowStepId,
            title,
            description,
            assigneeUserId,
            dueDate
        );

        ApplyStatus(entity, status);

        return await _serviceWorkflowTaskRepository.InsertAsync(entity, autoSave: true);
    }

    public async Task<ServiceWorkflowTask> UpdateAsync(
        Guid id,
        string title,
        string description,
        Guid? assigneeUserId,
        DateTime? dueDate,
        ServiceWorkflowTaskStatus status
    )
    {
        var entity = await _serviceWorkflowTaskRepository.GetAsync(id);

        entity.SetTitle(title);
        entity.SetDescription(description);
        entity.SetAssigneeUserId(assigneeUserId);
        entity.SetDueDate(dueDate);

        ApplyStatus(entity, status);

        return await _serviceWorkflowTaskRepository.UpdateAsync(entity, autoSave: true);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _serviceWorkflowTaskRepository.DeleteAsync(id, autoSave: true);
    }

    private static void ApplyStatus(ServiceWorkflowTask entity, ServiceWorkflowTaskStatus targetStatus)
    {
        switch (targetStatus)
        {
            case ServiceWorkflowTaskStatus.Pending:
                return;
            case ServiceWorkflowTaskStatus.InProgress:
                entity.Start();
                return;
            case ServiceWorkflowTaskStatus.Completed:
                entity.Complete();
                return;
            case ServiceWorkflowTaskStatus.Rejected:
                entity.Reject();
                return;
            case ServiceWorkflowTaskStatus.Cancelled:
                entity.Cancel();
                return;
            default:
                throw new BusinessException("Workflow:ServiceWorkflowTasks:InvalidStatus");
        }
    }
}
