using ResourceryPlatformWorkflow.Documents.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace ResourceryPlatformWorkflow.Documents;

public abstract class DocumentsController : AbpControllerBase
{
    protected DocumentsController()
    {
        LocalizationResource = typeof(DocumentsResource);
    }
}
