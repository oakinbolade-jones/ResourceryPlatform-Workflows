using ResourceryPlatformWorkflow.WebApp.Localization;
using Volo.Abp.AspNetCore.Components;

namespace ResourceryPlatformWorkflow.WebApp.Blazor.Client;

public abstract class WebAppComponentBase : AbpComponentBase
{
    protected WebAppComponentBase()
    {
        LocalizationResource = typeof(WebAppResource);
    }
}
