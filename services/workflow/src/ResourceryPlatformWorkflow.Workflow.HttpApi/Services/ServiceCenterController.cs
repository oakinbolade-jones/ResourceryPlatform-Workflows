using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace ResourceryPlatformWorkflow.Workflow.Services;

[Area(WorkflowRemoteServiceConsts.ModuleName)]
[RemoteService(Name = WorkflowRemoteServiceConsts.RemoteServiceName)]
[ApiExplorerSettings(IgnoreApi = false)]
[ApiController]
[Route("api/workflow/service-centers")]
public class ServiceCenterController(IServiceCenterAppService appService)
    : WorkflowController,
        IServiceCenterAppService
{
    private readonly IServiceCenterAppService _appService = appService;

    [HttpGet("{id}")]
    public Task<ServiceCenterDto> GetAsync(Guid id) => _appService.GetAsync(id);

    [HttpGet]
    public Task<List<ServiceCenterDto>> GetListAsync() => _appService.GetListAsync();

    [HttpPost]
    public Task<ServiceCenterDto> CreateAsync(CreateUpdateServiceCenterDto input) =>
        _appService.CreateAsync(input);

    [HttpPut("{id}")]
    public Task<ServiceCenterDto> UpdateAsync(Guid id, CreateUpdateServiceCenterDto input) =>
        _appService.UpdateAsync(id, input);

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id) => _appService.DeleteAsync(id);
}