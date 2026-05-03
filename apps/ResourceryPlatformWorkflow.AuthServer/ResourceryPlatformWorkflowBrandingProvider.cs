using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace ResourceryPlatformWorkflow;

[Dependency(ReplaceServices = true)]
public class ResourceryPlatformWorkflowBrandingProvider : DefaultBrandingProvider
{
<<<<<<< HEAD
    public override string AppName => "ResourceryPlatformWorkflow";
=======
    public override string AppName => "Resourcery Platform";
>>>>>>> refs/heads/development
}
