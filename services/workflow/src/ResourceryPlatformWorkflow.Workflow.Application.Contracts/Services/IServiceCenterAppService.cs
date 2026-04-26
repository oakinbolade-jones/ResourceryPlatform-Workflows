using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ResourceryPlatformWorkflow.Workflow.Services;

public interface IServiceCenterAppService : IApplicationService
{
    Task<ServiceCenterDto> GetAsync(Guid id);
    Task<List<ServiceCenterDto>> GetListAsync();
    Task<ServiceCenterDto> CreateAsync(CreateUpdateServiceCenterDto input);
    Task<ServiceCenterDto> UpdateAsync(Guid id, CreateUpdateServiceCenterDto input);
    Task DeleteAsync(Guid id);
}