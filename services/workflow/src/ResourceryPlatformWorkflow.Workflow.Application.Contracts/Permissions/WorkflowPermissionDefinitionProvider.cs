using ResourceryPlatformWorkflow.Workflow.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace ResourceryPlatformWorkflow.Workflow.Permissions;

public class WorkflowPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var projectsGroup = context.AddGroup(
            WorkflowPermissions.GroupName,
            L("Permission:Workflow")
        );
        var projectsPermissions = projectsGroup.AddPermission(
            WorkflowPermissions.Issues.Default,
            L("Permission:Workflow:Issues")
        );
        projectsPermissions.AddChild(
            WorkflowPermissions.Issues.Create,
            L("Permission:Workflow:Issues:Create")
        );
        projectsPermissions.AddChild(
            WorkflowPermissions.Issues.Update,
            L("Permission:Workflow:Issues:Update")
        );
        projectsPermissions.AddChild(
            WorkflowPermissions.Issues.Delete,
            L("Permission:Workflow:Issues:Delete")
        );
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<WorkflowResource>(name);
    }
}
