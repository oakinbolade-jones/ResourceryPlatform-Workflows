using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace ResourceryPlatformWorkflow.Workflow.Services;

[Area(WorkflowRemoteServiceConsts.ModuleName)]
[RemoteService(Name = WorkflowRemoteServiceConsts.RemoteServiceName)]
[Route("api/workflow/services")]
public class ServiceController(IServiceAppService appService) : WorkflowController, IServiceAppService
{
    private readonly IServiceAppService _appService = appService;

    [HttpGet("{id}")]
    public async Task<ServiceDto> GetAsync(Guid id)
    {
        return await _appService.GetAsync(id);
    }

    [HttpGet]
    public async Task<List<ServiceDto>> GetListAsync()
    {
        return await _appService.GetListAsync();
    }

    [HttpPost]
    public async Task<ServiceDto> CreateAsync(CreateUpdateServiceDto input)
    {
        return await _appService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public async Task<ServiceDto> UpdateAsync(Guid id, CreateUpdateServiceDto input)
    {
        return await _appService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        await _appService.DeleteAsync(id);
    }
}
