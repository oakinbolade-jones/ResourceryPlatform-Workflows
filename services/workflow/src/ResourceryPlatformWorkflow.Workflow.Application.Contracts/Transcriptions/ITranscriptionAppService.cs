using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ResourceryPlatformWorkflow.Workflow.Transcriptions;

public interface ITranscriptionAppService : IApplicationService
{
    Task<TranscriptionDto> GetAsync(Guid id);
    Task<List<TranscriptionDto>> GetListAsync();
    Task<TranscriptionDto> CreateAsync(CreateUpdateTranscriptionDto input);
    Task<TranscriptionDto> UpdateAsync(Guid id, CreateUpdateTranscriptionDto input);
    Task DeleteAsync(Guid id);
    Task<TranscriptionDto> GetBySourceReferenceIdAsync(string sourceReferenceId);
    Task<TranscriptionDto> SaveTranscriptAsync(string sourceReferenceId, string transcript);
}
