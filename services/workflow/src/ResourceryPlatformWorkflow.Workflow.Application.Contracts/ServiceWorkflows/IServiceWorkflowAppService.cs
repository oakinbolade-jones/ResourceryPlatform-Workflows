using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

public interface IServiceWorkflowAppService : IApplicationService
{
    Task<ServiceWorkflowDto> GetAsync(Guid id);
    Task<List<ServiceWorkflowDto>> GetListAsync();
    Task<ServiceWorkflowDto> CreateAsync(CreateUpdateServiceWorkflowDto input);
    Task<ServiceWorkflowDto> UpdateAsync(Guid id, CreateUpdateServiceWorkflowDto input);
    Task DeleteAsync(Guid id);
}
