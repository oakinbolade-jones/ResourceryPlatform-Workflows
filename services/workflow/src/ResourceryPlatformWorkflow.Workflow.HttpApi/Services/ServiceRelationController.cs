using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace ResourceryPlatformWorkflow.Workflow.Services;

[Area(WorkflowRemoteServiceConsts.ModuleName)]
[RemoteService(Name = WorkflowRemoteServiceConsts.RemoteServiceName)]
[Route("api/workflow/service-relations")]
public class ServiceRelationController(IServiceRelationAppService appService)
    : WorkflowController,
        IServiceRelationAppService
{
    private readonly IServiceRelationAppService _appService = appService;

    [HttpGet("{serviceWorkflowId}")]
    public Task<ServiceRelationDto> GetAsync(Guid serviceWorkflowId) =>
        _appService.GetAsync(serviceWorkflowId);

    [HttpGet]
    public Task<List<ServiceRelationDto>> GetListAsync() => _appService.GetListAsync();

    [HttpPost]
    public Task<ServiceRelationDto> CreateAsync(CreateUpdateServiceRelationDto input) =>
        _appService.CreateAsync(input);

    [HttpPut("{serviceWorkflowId}")]
    public Task<ServiceRelationDto> UpdateAsync(
        Guid serviceWorkflowId,
        CreateUpdateServiceRelationDto input
    ) => _appService.UpdateAsync(serviceWorkflowId, input);

    [HttpDelete("{serviceWorkflowId}")]
    public Task DeleteAsync(Guid serviceWorkflowId) => _appService.DeleteAsync(serviceWorkflowId);
}
