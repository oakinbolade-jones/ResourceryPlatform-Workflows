using ResourceryPlatformWorkflow.IdentityService.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace ResourceryPlatformWorkflow.IdentityService;

public abstract class IdentityServiceController : AbpControllerBase
{
    protected IdentityServiceController()
    {
        LocalizationResource = typeof(IdentityServiceResource);
    }
}
