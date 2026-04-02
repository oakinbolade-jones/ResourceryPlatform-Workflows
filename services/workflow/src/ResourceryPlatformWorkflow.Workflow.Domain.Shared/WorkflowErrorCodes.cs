namespace ResourceryPlatformWorkflow.Workflow;

public static class WorkflowErrorCodes
{
    public static class Requests
    {
        public const string DuplicateDocument = "Workflow:Requests:DuplicateDocument";
        public const string RequestNotFound = "Workflow:Requests:RequestNotFound";
        public const string InvalidRequestStatus = "Workflow:Requests:InvalidRequestStatus";
    }

    public static class Services
    {
        public const string ServiceNotFound = "Workflow:Services:ServiceNotFound";
    }

    public static class ServiceWorkflows
    {
        public const string InvalidStepOrder = "Workflow:ServiceWorkflows:InvalidStepOrder";
        public const string DuplicateStepOrder = "Workflow:ServiceWorkflows:DuplicateStepOrder";
    }
}
