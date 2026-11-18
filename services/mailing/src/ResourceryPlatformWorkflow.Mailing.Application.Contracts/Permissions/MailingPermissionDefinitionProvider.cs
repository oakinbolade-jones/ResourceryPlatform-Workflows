using ResourceryPlatformWorkflow.Mailing.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace ResourceryPlatformWorkflow.Mailing.Permissions;

public class MailingPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var projectsGroup = context.AddGroup(
            MailingPermissions.GroupName,
            L("Permission:Mailing")
        );
        var projectsPermissions = projectsGroup.AddPermission(
            MailingPermissions.Issues.Default,
            L("Permission:Mailing:Issues")
        );
        projectsPermissions.AddChild(
            MailingPermissions.Issues.Create,
            L("Permission:Mailing:Issues:Create")
        );
        projectsPermissions.AddChild(
            MailingPermissions.Issues.Update,
            L("Permission:Mailing:Issues:Update")
        );
        projectsPermissions.AddChild(
            MailingPermissions.Issues.Delete,
            L("Permission:Mailing:Issues:Delete")
        );
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<MailingResource>(name);
    }
}
