using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

public interface IServiceWorkflowHistoryAppService : IApplicationService
{
    Task<ServiceWorkflowHistoryDto> GetAsync(Guid id);
    Task<List<ServiceWorkflowHistoryDto>> GetListAsync();
    Task<ServiceWorkflowHistoryDto> CreateAsync(CreateUpdateServiceWorkflowHistoryDto input);
    Task<ServiceWorkflowHistoryDto> UpdateAsync(Guid id, CreateUpdateServiceWorkflowHistoryDto input);
    Task DeleteAsync(Guid id);
}
