using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

public class ServiceWorkflowManager : DomainService
{
    private readonly IRepository<ServiceWorkflow, Guid> _serviceWorkflowRepository;

    public ServiceWorkflowManager(IRepository<ServiceWorkflow, Guid> serviceWorkflowRepository)
    {
        _serviceWorkflowRepository = serviceWorkflowRepository;
    }

    public async Task<ServiceWorkflow> CreateAsync(
        Guid id,
        string name,
        string code,
        string displayName,
        string leadTime,
        string leadTimeType,
        string description,
        bool isActive,
        IEnumerable<ServiceWorkflowStep> steps
    )
    {
        var entity = new ServiceWorkflow(id, name, code, displayName, leadTime, leadTimeType, description);
        entity.SetIsActive(isActive);

        if (steps != null)
        {
            foreach (var step in steps.OrderBy(x => x.Order))
            {
                entity.AddStep(
                    step.Id,
                    step.Name,
                    step.Code,
                    step.Description,
                    step.Order,
                    step.DisplayName,
                    step.DisplayNameOutput,
                    step.Output,
                    step.TATType,
                    step.TATUnit
                );
            }
        }

        return await _serviceWorkflowRepository.InsertAsync(entity, autoSave: true);
    }

    public async Task<ServiceWorkflow> UpdateAsync(
        Guid id,
        string name,
        string code,
        string displayName,
        string leadTime,
        string leadTimeType,
        string description,
        bool isActive,
        IEnumerable<ServiceWorkflowStep> steps
    )
    {
        var entity = await _serviceWorkflowRepository.GetAsync(id, includeDetails: true);

        entity.SetName(name);
        entity.SetCode(code);
        entity.SetDisplayName(displayName);
        entity.SetLeadTime(leadTime);
        entity.SetLeadTimeType(leadTimeType);
        entity.SetDescription(description);
        entity.SetIsActive(isActive);

        entity.Steps.Clear();

        if (steps != null)
        {
            foreach (var step in steps.OrderBy(x => x.Order))
            {
                entity.AddStep(
                    step.Id,
                    step.Name,
                    step.Code,
                    step.Description,
                    step.Order,
                    step.DisplayName,
                    step.DisplayNameOutput,
                    step.Output,
                    step.TATType,
                    step.TATUnit
                );
            }
        }

        return await _serviceWorkflowRepository.UpdateAsync(entity, autoSave: true);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _serviceWorkflowRepository.DeleteAsync(id, autoSave: true);
    }
}
