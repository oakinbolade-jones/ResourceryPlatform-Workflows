using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

public interface IServiceWorkflowStepAppService : IApplicationService
{
    Task<ServiceWorkflowStepDto> GetAsync(Guid id);
    Task<List<ServiceWorkflowStepDto>> GetListAsync();
    Task<ServiceWorkflowStepDto> CreateAsync(CreateUpdateServiceWorkflowStepDto input);
    Task<ServiceWorkflowStepDto> UpdateAsync(Guid id, CreateUpdateServiceWorkflowStepDto input);
    Task DeleteAsync(Guid id);
}
