using ResourceryPlatformWorkflow.Administration.Localization;
using Volo.Abp.Application.Services;

namespace ResourceryPlatformWorkflow.Administration;

public abstract class AdministrationAppService : ApplicationService
{
    protected AdministrationAppService()
    {
        LocalizationResource = typeof(AdministrationResource);
        ObjectMapperContext = typeof(AdministrationApplicationModule);
    }
}
