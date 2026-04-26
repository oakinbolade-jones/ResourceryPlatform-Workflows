using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

[Area(WorkflowRemoteServiceConsts.ModuleName)]
[RemoteService(Name = WorkflowRemoteServiceConsts.RemoteServiceName)]
[Route("api/workflow/service-workflow-steps")]
public class ServiceWorkflowStepController(IServiceWorkflowStepAppService appService)
    : WorkflowController,
        IServiceWorkflowStepAppService
{
    private readonly IServiceWorkflowStepAppService _appService = appService;

    [HttpGet("{id}")]
    public async Task<ServiceWorkflowStepDto> GetAsync(Guid id)
    {
        return await _appService.GetAsync(id);
    }

    [HttpGet]
    public async Task<List<ServiceWorkflowStepDto>> GetListAsync()
    {
        return await _appService.GetListAsync();
    }

    [HttpPost]
    public async Task<ServiceWorkflowStepDto> CreateAsync(CreateUpdateServiceWorkflowStepDto input)
    {
        return await _appService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public async Task<ServiceWorkflowStepDto> UpdateAsync(Guid id, CreateUpdateServiceWorkflowStepDto input)
    {
        return await _appService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        await _appService.DeleteAsync(id);
    }
}
