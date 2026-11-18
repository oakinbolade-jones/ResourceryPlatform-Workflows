using ResourceryPlatformWorkflow.Documents.Localization;
using Volo.Abp.Application.Services;

namespace ResourceryPlatformWorkflow.Documents;

public abstract class DocumentsAppService : ApplicationService
{
    protected DocumentsAppService()
    {
        LocalizationResource = typeof(DocumentsResource);
        ObjectMapperContext = typeof(DocumentsApplicationModule);
    }
}
