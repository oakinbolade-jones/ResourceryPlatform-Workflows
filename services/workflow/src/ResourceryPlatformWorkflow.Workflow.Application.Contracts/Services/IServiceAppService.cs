using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ResourceryPlatformWorkflow.Workflow.Services;

public interface IServiceAppService : IApplicationService
{
    Task<ServiceDto> GetAsync(Guid id);
    Task<List<ServiceDto>> GetListAsync();
    Task<ServiceDto> CreateAsync(CreateUpdateServiceDto input);
    Task<ServiceDto> UpdateAsync(Guid id, CreateUpdateServiceDto input);
    Task DeleteAsync(Guid id);
}
