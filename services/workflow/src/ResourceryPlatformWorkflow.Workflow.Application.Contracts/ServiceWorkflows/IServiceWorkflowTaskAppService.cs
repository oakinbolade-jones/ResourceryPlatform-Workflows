using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;

public interface IServiceWorkflowTaskAppService : IApplicationService
{
    Task<ServiceWorkflowTaskDto> GetAsync(Guid id);
    Task<List<ServiceWorkflowTaskDto>> GetListAsync();
    Task<ServiceWorkflowTaskDto> CreateAsync(CreateUpdateServiceWorkflowTaskDto input);
    Task<ServiceWorkflowTaskDto> UpdateAsync(Guid id, CreateUpdateServiceWorkflowTaskDto input);
    Task DeleteAsync(Guid id);
}
