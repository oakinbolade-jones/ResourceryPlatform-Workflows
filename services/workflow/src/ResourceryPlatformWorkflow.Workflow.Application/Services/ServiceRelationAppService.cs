using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ResourceryPlatformWorkflow.Workflow.Permissions;
using ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace ResourceryPlatformWorkflow.Workflow.Services;

[Authorize(WorkflowPermissions.ServiceRelations.Default)]
public class ServiceRelationAppService(
    IRepository<Service, Guid> serviceRepository,
    IRepository<ServiceWorkflow, Guid> serviceWorkflowRepository
) : WorkflowAppService, IServiceRelationAppService
{
    private readonly IRepository<Service, Guid> _serviceRepository = serviceRepository;
    private readonly IRepository<ServiceWorkflow, Guid> _serviceWorkflowRepository =
        serviceWorkflowRepository;

    public async Task<ServiceRelationDto> GetAsync(Guid serviceWorkflowId)
    {
        var workflow = await _serviceWorkflowRepository.GetAsync(serviceWorkflowId);
        return Map(workflow.ServiceRelation);
    }

    public async Task<List<ServiceRelationDto>> GetListAsync()
    {
        var queryable = await _serviceWorkflowRepository.GetQueryableAsync();

        return queryable
            .Where(x => x.ServiceRelation != null && x.ServiceRelation.ServiceId != Guid.Empty)
            .Select(x => x.ServiceRelation)
            .AsEnumerable()
            .Select(Map)
            .ToList();
    }

    [Authorize(WorkflowPermissions.ServiceRelations.Create)]
    public async Task<ServiceRelationDto> CreateAsync(CreateUpdateServiceRelationDto input)
    {
        Check.NotNull(input, nameof(input));

        await _serviceRepository.GetAsync(input.ServiceId);

        var workflow = await _serviceWorkflowRepository.GetAsync(input.ServiceWorkflowId);
        workflow.SetService(input.ServiceId);

        workflow = await _serviceWorkflowRepository.UpdateAsync(workflow, autoSave: true);
        return Map(workflow.ServiceRelation);
    }

    [Authorize(WorkflowPermissions.ServiceRelations.Update)]
    public async Task<ServiceRelationDto> UpdateAsync(
        Guid serviceWorkflowId,
        CreateUpdateServiceRelationDto input
    )
    {
        Check.NotNull(input, nameof(input));

        await _serviceRepository.GetAsync(input.ServiceId);

        var workflow = await _serviceWorkflowRepository.GetAsync(serviceWorkflowId);
        workflow.SetService(input.ServiceId);

        workflow = await _serviceWorkflowRepository.UpdateAsync(workflow, autoSave: true);
        return Map(workflow.ServiceRelation);
    }

    [Authorize(WorkflowPermissions.ServiceRelations.Delete)]
    public async Task DeleteAsync(Guid serviceWorkflowId)
    {
        var workflow = await _serviceWorkflowRepository.GetAsync(serviceWorkflowId);
        workflow.SetService(Guid.Empty);

        await _serviceWorkflowRepository.UpdateAsync(workflow, autoSave: true);
    }

    private static ServiceRelationDto Map(ServiceRelation relation)
    {
        return new ServiceRelationDto
        {
            ServiceId = relation?.ServiceId ?? Guid.Empty,
            ServiceWorkflowId = relation?.ServiceWorkflowId ?? Guid.Empty
        };
    }
}
