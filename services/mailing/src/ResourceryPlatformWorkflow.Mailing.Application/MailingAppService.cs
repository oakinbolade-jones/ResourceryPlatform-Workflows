using ResourceryPlatformWorkflow.Mailing.Localization;
using Volo.Abp.Application.Services;

namespace ResourceryPlatformWorkflow.Mailing;

public abstract class MailingAppService : ApplicationService
{
    protected MailingAppService()
    {
        LocalizationResource = typeof(MailingResource);
        ObjectMapperContext = typeof(MailingApplicationModule);
    }
}
