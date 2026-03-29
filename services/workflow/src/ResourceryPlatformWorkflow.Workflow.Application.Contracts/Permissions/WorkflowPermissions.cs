using Volo.Abp.Reflection;

namespace ResourceryPlatformWorkflow.Workflow.Permissions;

public class WorkflowPermissions
{
    public const string GroupName = "Workflows";

    public static class Services
    {
        public const string Default = GroupName + ".Services";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class ServiceRelations
    {
        public const string Default = GroupName + ".ServiceRelations";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class Requests
    {
        public const string Default = GroupName + ".Requests";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class RequestDocuments
    {
        public const string Default = GroupName + ".RequestDocuments";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class ServiceWorkflows
    {
        public const string Default = GroupName + ".ServiceWorkflows";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class ServiceWorkflowSteps
    {
        public const string Default = GroupName + ".ServiceWorkflowSteps";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class ServiceWorkflowInstances
    {
        public const string Default = GroupName + ".ServiceWorkflowInstances";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class ServiceWorkflowTasks
    {
        public const string Default = GroupName + ".ServiceWorkflowTasks";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class ServiceWorkflowHistory
    {
        public const string Default = GroupName + ".ServiceWorkflowHistory";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(WorkflowPermissions));
    }
}
