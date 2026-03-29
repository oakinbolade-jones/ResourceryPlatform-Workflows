using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

[Area(WorkflowRemoteServiceConsts.ModuleName)]
[RemoteService(Name = WorkflowRemoteServiceConsts.RemoteServiceName)]
[Route("api/workflow/service-workflow-history")]
public class ServiceWorkflowHistoryController(IServiceWorkflowHistoryAppService appService)
    : WorkflowController,
        IServiceWorkflowHistoryAppService
{
    private readonly IServiceWorkflowHistoryAppService _appService = appService;

    [HttpGet("{id}")]
    public async Task<ServiceWorkflowHistoryDto> GetAsync(Guid id)
    {
        return await _appService.GetAsync(id);
    }

    [HttpGet]
    public async Task<List<ServiceWorkflowHistoryDto>> GetListAsync()
    {
        return await _appService.GetListAsync();
    }

    [HttpPost]
    public async Task<ServiceWorkflowHistoryDto> CreateAsync(CreateUpdateServiceWorkflowHistoryDto input)
    {
        return await _appService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public async Task<ServiceWorkflowHistoryDto> UpdateAsync(
        Guid id,
        CreateUpdateServiceWorkflowHistoryDto input
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
