using Volo.Abp.Reflection;

namespace ResourceryPlatformWorkflow.Workflow.Permissions;

public class WorkflowPermissions
{
    public const string GroupName = "Workflow";

    public static class Issues
    {
        public const string Default = GroupName + ".Issues";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(WorkflowPermissions));
    }
}
