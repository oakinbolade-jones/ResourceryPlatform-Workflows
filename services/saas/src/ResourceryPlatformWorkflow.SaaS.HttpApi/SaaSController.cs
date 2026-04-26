using ResourceryPlatformWorkflow.SaaS.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace ResourceryPlatformWorkflow.SaaS;

public abstract class SaaSController : AbpControllerBase
{
    protected SaaSController()
    {
        LocalizationResource = typeof(SaaSResource);
    }
}
