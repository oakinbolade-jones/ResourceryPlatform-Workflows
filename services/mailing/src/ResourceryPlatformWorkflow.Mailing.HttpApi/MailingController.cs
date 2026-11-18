using ResourceryPlatformWorkflow.Mailing.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace ResourceryPlatformWorkflow.Mailing;

public abstract class MailingController : AbpControllerBase
{
    protected MailingController()
    {
        LocalizationResource = typeof(MailingResource);
    }
}
