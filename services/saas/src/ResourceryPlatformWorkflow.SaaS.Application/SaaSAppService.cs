using ResourceryPlatformWorkflow.SaaS.Localization;
using Volo.Abp.Application.Services;

namespace ResourceryPlatformWorkflow.SaaS;

public abstract class SaaSAppService : ApplicationService
{
    protected SaaSAppService()
    {
        LocalizationResource = typeof(SaaSResource);
        ObjectMapperContext = typeof(SaaSApplicationModule);
    }
}
