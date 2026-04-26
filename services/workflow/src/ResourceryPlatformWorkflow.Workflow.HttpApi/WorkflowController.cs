using ResourceryPlatformWorkflow.Workflow.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace ResourceryPlatformWorkflow.Workflow;

public abstract class WorkflowController : AbpControllerBase
{
    protected WorkflowController()
    {
        LocalizationResource = typeof(WorkflowResource);
    }
}
