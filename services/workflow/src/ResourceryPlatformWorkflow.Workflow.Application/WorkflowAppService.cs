using ResourceryPlatformWorkflow.Workflow.Localization;
using Volo.Abp.Application.Services;

namespace ResourceryPlatformWorkflow.Workflow;

public abstract class WorkflowAppService : ApplicationService
{
    protected WorkflowAppService()
    {
        LocalizationResource = typeof(WorkflowResource);
        ObjectMapperContext = typeof(WorkflowApplicationModule);
    }
}
