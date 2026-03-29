using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ResourceryPlatformWorkflow.Workflow.Requests;

public interface IRequestAppService : IApplicationService
{
    Task<RequestDto> GetAsync(Guid id);
    Task<List<RequestDto>> GetListAsync();
    Task<List<RequestDto>> GetByStatusAsync(RequestStatus requestStatus);
    Task<List<RequestDto>> GetByUserAsync(Guid userId);
    Task<List<RequestDto>> GetByTypeAsync(RequestType requestType);

    Task<RequestDto> CreateAsync(CreateUpdateRequestDto input);
    Task<RequestDto> UpdateAsync(Guid id, CreateUpdateRequestDto input);
    Task<RequestDto> AddDocumentsAsync(Guid id, List<CreateUpdateRequestDocumentDto> documents);
    Task DeleteAsync(Guid id);
}
