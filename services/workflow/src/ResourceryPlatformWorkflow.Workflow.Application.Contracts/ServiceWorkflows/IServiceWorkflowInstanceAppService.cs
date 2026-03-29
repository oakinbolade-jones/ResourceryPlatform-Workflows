using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

public interface IServiceWorkflowInstanceAppService : IApplicationService
{
    Task<ServiceWorkflowInstanceDto> GetAsync(Guid id);
    Task<List<ServiceWorkflowInstanceDto>> GetListAsync();
    Task<ServiceWorkflowInstanceDto> CreateAsync(CreateUpdateServiceWorkflowInstanceDto input);
    Task<ServiceWorkflowInstanceDto> UpdateAsync(Guid id, CreateUpdateServiceWorkflowInstanceDto input);
    Task DeleteAsync(Guid id);
}
