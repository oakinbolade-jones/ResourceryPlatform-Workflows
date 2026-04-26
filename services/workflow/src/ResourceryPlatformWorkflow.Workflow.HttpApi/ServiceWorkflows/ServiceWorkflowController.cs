using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

[Area(WorkflowRemoteServiceConsts.ModuleName)]
[RemoteService(Name = WorkflowRemoteServiceConsts.RemoteServiceName)]
[Route("api/workflow/service-workflows")]
public class ServiceWorkflowController(IServiceWorkflowAppService appService)
    : WorkflowController,
        IServiceWorkflowAppService
{
    private readonly IServiceWorkflowAppService _appService = appService;

    [HttpGet("{id}")]
    public async Task<ServiceWorkflowDto> GetAsync(Guid id)
    {
        return await _appService.GetAsync(id);
    }

    [HttpGet]
    public async Task<List<ServiceWorkflowDto>> GetListAsync()
    {
        return await _appService.GetListAsync();
    }

    [HttpPost]
    public async Task<ServiceWorkflowDto> CreateAsync(CreateUpdateServiceWorkflowDto input)
    {
        return await _appService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public async Task<ServiceWorkflowDto> UpdateAsync(Guid id, CreateUpdateServiceWorkflowDto input)
    {
        return await _appService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        await _appService.DeleteAsync(id);
    }
}
