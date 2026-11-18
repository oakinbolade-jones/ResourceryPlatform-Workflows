using ResourceryPlatformWorkflow.Documents.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace ResourceryPlatformWorkflow.Documents.Permissions;

public class DocumentsPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var projectsGroup = context.AddGroup(
            DocumentsPermissions.GroupName,
            L("Permission:Documents")
        );
        var projectsPermissions = projectsGroup.AddPermission(
            DocumentsPermissions.Issues.Default,
            L("Permission:Documents:Issues")
        );
        projectsPermissions.AddChild(
            DocumentsPermissions.Issues.Create,
            L("Permission:Documents:Issues:Create")
        );
        projectsPermissions.AddChild(
            DocumentsPermissions.Issues.Update,
            L("Permission:Documents:Issues:Update")
        );
        projectsPermissions.AddChild(
            DocumentsPermissions.Issues.Delete,
            L("Permission:Documents:Issues:Delete")
        );
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<DocumentsResource>(name);
    }
}
