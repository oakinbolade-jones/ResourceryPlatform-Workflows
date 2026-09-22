namespace ResourceryPlatformWorkflow.Workflow;

public static class WorkflowErrorCodes
{
    public static class Requests
    {
        public const string DuplicateDocument = "Workflow:Requests:DuplicateDocument";
        public const string RequestNotFound = "Workflow:Requests:RequestNotFound";
        public const string InvalidRequestStatus = "Workflow:Requests:InvalidRequestStatus";
        public const string RequestDataRequired = "Workflow:Requests:RequestDataRequired";
        public const string MeetingDataRequired = "Workflow:Requests:MeetingDataRequired";
        public const string MeetingItemDataRequired = "Workflow:Requests:MeetingItemDataRequired";
        public const string InvalidMeetingRequirement = "Workflow:Requests:InvalidMeetingRequirement";
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

    public static class Transcriptions
    {
        public const string TranscriptionNotFound = "Workflow:Transcriptions:TranscriptionNotFound";
    }
}
