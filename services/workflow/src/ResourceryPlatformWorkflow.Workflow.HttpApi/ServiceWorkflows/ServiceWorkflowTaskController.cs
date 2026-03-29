using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

[Area(WorkflowRemoteServiceConsts.ModuleName)]
[RemoteService(Name = WorkflowRemoteServiceConsts.RemoteServiceName)]
[Route("api/workflow/service-workflow-tasks")]
public class ServiceWorkflowTaskController(IServiceWorkflowTaskAppService appService)
    : WorkflowController,
        IServiceWorkflowTaskAppService
{
    private readonly IServiceWorkflowTaskAppService _appService = appService;

    [HttpGet("{id}")]
    public async Task<ServiceWorkflowTaskDto> GetAsync(Guid id)
    {
        return await _appService.GetAsync(id);
    }

    [HttpGet]
    public async Task<List<ServiceWorkflowTaskDto>> GetListAsync()
    {
        return await _appService.GetListAsync();
    }

    [HttpPost]
    public async Task<ServiceWorkflowTaskDto> CreateAsync(CreateUpdateServiceWorkflowTaskDto input)
    {
        return await _appService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public async Task<ServiceWorkflowTaskDto> UpdateAsync(Guid id, CreateUpdateServiceWorkflowTaskDto input)
    {
        return await _appService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        await _appService.DeleteAsync(id);
    }
}
