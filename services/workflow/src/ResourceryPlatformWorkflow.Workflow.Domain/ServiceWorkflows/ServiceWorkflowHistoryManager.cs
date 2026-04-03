using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

public class ServiceWorkflowHistoryManager : DomainService
{
    private readonly IRepository<ServiceWorkflowHistory, Guid> _serviceWorkflowHistoryRepository;

    public ServiceWorkflowHistoryManager(IRepository<ServiceWorkflowHistory, Guid> serviceWorkflowHistoryRepository)
    {
        _serviceWorkflowHistoryRepository = serviceWorkflowHistoryRepository;
    }

    public async Task<ServiceWorkflowHistory> CreateAsync(
        Guid id,
        Guid serviceWorkflowInstanceId,
        ServiceWorkflowHistoryType type,
        string action,
        string comment,
        Guid? performedByUserId,
        Guid? serviceWorkflowStepId = null,
        Guid? serviceWorkflowTaskId = null
    )
    {
        var entity = new ServiceWorkflowHistory(
            id,
            serviceWorkflowInstanceId,
            type,
            action,
            comment,
            performedByUserId,
            serviceWorkflowStepId,
            serviceWorkflowTaskId
        );

        return await _serviceWorkflowHistoryRepository.InsertAsync(entity, autoSave: true);
    }

    public async Task<ServiceWorkflowHistory> UpdateAsync(
        Guid id,
        string action,
        string comment
    )
    {
        var entity = await _serviceWorkflowHistoryRepository.GetAsync(id);

        entity.SetAction(action);
        entity.SetComment(comment);

        return await _serviceWorkflowHistoryRepository.UpdateAsync(entity, autoSave: true);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _serviceWorkflowHistoryRepository.DeleteAsync(id, autoSave: true);
    }
}
