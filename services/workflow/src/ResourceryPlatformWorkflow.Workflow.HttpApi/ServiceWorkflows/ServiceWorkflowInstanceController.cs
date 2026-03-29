using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

[Area(WorkflowRemoteServiceConsts.ModuleName)]
[RemoteService(Name = WorkflowRemoteServiceConsts.RemoteServiceName)]
[Route("api/workflow/service-workflow-instances")]
public class ServiceWorkflowInstanceController(IServiceWorkflowInstanceAppService appService)
    : WorkflowController,
        IServiceWorkflowInstanceAppService
{
    private readonly IServiceWorkflowInstanceAppService _appService = appService;

    [HttpGet("{id}")]
    public async Task<ServiceWorkflowInstanceDto> GetAsync(Guid id)
    {
        return await _appService.GetAsync(id);
    }

    [HttpGet]
    public async Task<List<ServiceWorkflowInstanceDto>> GetListAsync()
    {
        return await _appService.GetListAsync();
    }

    [HttpPost]
    public async Task<ServiceWorkflowInstanceDto> CreateAsync(CreateUpdateServiceWorkflowInstanceDto input)
    {
        return await _appService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public async Task<ServiceWorkflowInstanceDto> UpdateAsync(
        Guid id,
        CreateUpdateServiceWorkflowInstanceDto input
    )
    {
        return await _appService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        await _appService.DeleteAsync(id);
    }
}
