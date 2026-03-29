using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ResourceryPlatformWorkflow.Workflow.Services;

public interface IServiceRelationAppService : IApplicationService
{
    Task<ServiceRelationDto> GetAsync(Guid serviceWorkflowId);
    Task<List<ServiceRelationDto>> GetListAsync();
    Task<ServiceRelationDto> CreateAsync(CreateUpdateServiceRelationDto input);
    Task<ServiceRelationDto> UpdateAsync(Guid serviceWorkflowId, CreateUpdateServiceRelationDto input);
    Task DeleteAsync(Guid serviceWorkflowId);
}
