using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace ResourceryPlatformWorkflow.Workflow.Requests;

[Area(WorkflowRemoteServiceConsts.ModuleName)]
[RemoteService(Name = WorkflowRemoteServiceConsts.RemoteServiceName)]
[Route("api/workflow/requests")]
public class RequestController(IRequestAppService requestAppService)
    : WorkflowController,
        IRequestAppService
{
    private readonly IRequestAppService _requestAppService = requestAppService;

    [HttpGet("{id}")]
    public async Task<RequestDto> GetAsync(Guid id)
    {
        return await _requestAppService.GetAsync(id);
    }

    [HttpGet]
    public async Task<List<RequestDto>> GetListAsync()
    {
        return await _requestAppService.GetListAsync();
    }

    [HttpGet("status/{requestStatus}")]
    public async Task<List<RequestDto>> GetByStatusAsync(RequestStatus requestStatus)
    {
        return await _requestAppService.GetByStatusAsync(requestStatus);
    }

    [HttpGet("user/{userId}")]
    public async Task<List<RequestDto>> GetByUserAsync(Guid userId)
    {
        return await _requestAppService.GetByUserAsync(userId);
    }

    [HttpGet("type/{requestType}")]
    public async Task<List<RequestDto>> GetByTypeAsync(RequestType requestType)
    {
        return await _requestAppService.GetByTypeAsync(requestType);
    }

    [HttpPost]
    public async Task<RequestDto> CreateAsync(CreateUpdateRequestDto input)
    {
        return await _requestAppService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public async Task<RequestDto> UpdateAsync(Guid id, CreateUpdateRequestDto input)
    {
        return await _requestAppService.UpdateAsync(id, input);
    }

    [HttpPost("{id}/documents")]
    public async Task<RequestDto> AddDocumentsAsync(
        Guid id,
        List<CreateUpdateRequestDocumentDto> documents
    )
    {
        return await _requestAppService.AddDocumentsAsync(id, documents);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        await _requestAppService.DeleteAsync(id);
    }
}
