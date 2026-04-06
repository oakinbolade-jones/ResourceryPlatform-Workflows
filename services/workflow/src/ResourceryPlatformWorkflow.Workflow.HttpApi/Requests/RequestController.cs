using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using ResourceryPlatformWorkflow.Workflow.Permissions;
using ResourceryPlatformWorkflow.Workflow.Requests;
using System.Collections.Generic;

namespace ResourceryPlatformWorkflow.Workflow.HttpApi.Requests;

[ApiController]
[Route("api/app/requests")]
public class RequestController : WorkflowController
{
    private readonly IRequestAppService _requestAppService;

    public RequestController(IRequestAppService requestAppService)
    {
        _requestAppService = requestAppService;
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<RequestDto> GetAsync(Guid id)
    {
        return _requestAppService.GetAsync(id);
    }

    [HttpGet]
    public virtual Task<List<RequestDto>> GetListAsync()
    {
        return _requestAppService.GetListAsync();
    }

    [HttpPost]
    public virtual Task<RequestDto> CreateAsync(CreateUpdateRequestDto input)
    {
        return _requestAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<RequestDto> UpdateAsync(Guid id, CreateUpdateRequestDto input)
    {
        return _requestAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _requestAppService.DeleteAsync(id);
    }
}
