using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

public class ServiceWorkflowInstanceManager : DomainService
{
    private readonly IRepository<ServiceWorkflowInstance, Guid> _serviceWorkflowInstanceRepository;

    public ServiceWorkflowInstanceManager(IRepository<ServiceWorkflowInstance, Guid> serviceWorkflowInstanceRepository)
    {
        _serviceWorkflowInstanceRepository = serviceWorkflowInstanceRepository;
    }

    public async Task<ServiceWorkflowInstance> CreateAsync(
        Guid id,
        Guid serviceWorkflowId,
        Guid requestId,
        Guid? currentStepId,
        ServiceWorkflowInstanceStatus status
    )
    {
        var entity = new ServiceWorkflowInstance(id, serviceWorkflowId, requestId);

        if (currentStepId.HasValue)
        {
            entity.StartStep(currentStepId.Value);
        }

        ApplyStatus(entity, status);

        return await _serviceWorkflowInstanceRepository.InsertAsync(entity, autoSave: true);
    }

    public async Task<ServiceWorkflowInstance> UpdateAsync(
        Guid id,
        Guid? currentStepId,
        ServiceWorkflowInstanceStatus status
    )
    {
        var entity = await _serviceWorkflowInstanceRepository.GetAsync(id);

        if (currentStepId.HasValue)
        {
            entity.StartStep(currentStepId.Value);
        }

        ApplyStatus(entity, status);

        return await _serviceWorkflowInstanceRepository.UpdateAsync(entity, autoSave: true);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _serviceWorkflowInstanceRepository.DeleteAsync(id, autoSave: true);
    }

    private static void ApplyStatus(ServiceWorkflowInstance entity, ServiceWorkflowInstanceStatus targetStatus)
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
}
