using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace ResourceryPlatformWorkflow;

[Dependency(ReplaceServices = true)]
public class ResourceryPlatformWorkflowBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "Resourcery Platform";
}
