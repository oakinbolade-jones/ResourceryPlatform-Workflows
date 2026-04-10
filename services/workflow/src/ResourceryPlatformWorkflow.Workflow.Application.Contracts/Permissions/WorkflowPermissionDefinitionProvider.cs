using ResourceryPlatformWorkflow.Workflow.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace ResourceryPlatformWorkflow.Workflow.Permissions;

public class WorkflowPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var workflowsGroup = context.AddGroup(
            WorkflowPermissions.GroupName,
            L("Permission:Workflow")
        );
        var serviceCentersPermissions = workflowsGroup.AddPermission(
            WorkflowPermissions.ServiceCenters.Default,
            L("Permission:Workflow:ServiceCenters")
        );
        serviceCentersPermissions.AddChild(
            WorkflowPermissions.ServiceCenters.Create,
            L("Permission:Workflow:ServiceCenters:Create")
        );
        serviceCentersPermissions.AddChild(
            WorkflowPermissions.ServiceCenters.Update,
            L("Permission:Workflow:ServiceCenters:Update")
        );
        serviceCentersPermissions.AddChild(
            WorkflowPermissions.ServiceCenters.Delete,
            L("Permission:Workflow:ServiceCenters:Delete")
        );

        var servicesPermissions = workflowsGroup.AddPermission(
            WorkflowPermissions.Services.Default,
            L("Permission:Workflow:Services")
        );
        servicesPermissions.AddChild(
            WorkflowPermissions.Services.Create,
            L("Permission:Workflow:Services:Create")
        );
        servicesPermissions.AddChild(
            WorkflowPermissions.Services.Update,
            L("Permission:Workflow:Services:Update")
        );
        servicesPermissions.AddChild(
            WorkflowPermissions.Services.Delete,
            L("Permission:Workflow:Services:Delete")
        );

        var projectsPermissions = workflowsGroup.AddPermission(
            WorkflowPermissions.Requests.Default,
            L("Permission:Workflow:Requests")
        );
        projectsPermissions.AddChild(
            WorkflowPermissions.Requests.Create,
            L("Permission:Workflow:Requests:Create")
        );
        projectsPermissions.AddChild(
            WorkflowPermissions.Requests.Update,
            L("Permission:Workflow:Requests:Update")
        );
        projectsPermissions.AddChild(
            WorkflowPermissions.Requests.Delete,
            L("Permission:Workflow:Requests:Delete")
        );

        var requestDocumentsPermissions = workflowsGroup.AddPermission(
            WorkflowPermissions.RequestDocuments.Default,
            L("Permission:Workflow:RequestDocuments")
        );
        requestDocumentsPermissions.AddChild(
            WorkflowPermissions.RequestDocuments.Create,
            L("Permission:Workflow:RequestDocuments:Create")
        );
        requestDocumentsPermissions.AddChild(
            WorkflowPermissions.RequestDocuments.Update,
            L("Permission:Workflow:RequestDocuments:Update")
        );
        requestDocumentsPermissions.AddChild(
            WorkflowPermissions.RequestDocuments.Delete,
            L("Permission:Workflow:RequestDocuments:Delete")
        );

        var serviceWorkflowsPermissions = workflowsGroup.AddPermission(
            WorkflowPermissions.ServiceWorkflows.Default,
            L("Permission:Workflow:ServiceWorkflows")
        );
        serviceWorkflowsPermissions.AddChild(
            WorkflowPermissions.ServiceWorkflows.Create,
            L("Permission:Workflow:ServiceWorkflows:Create")
        );
        serviceWorkflowsPermissions.AddChild(
            WorkflowPermissions.ServiceWorkflows.Update,
            L("Permission:Workflow:ServiceWorkflows:Update")
        );
        serviceWorkflowsPermissions.AddChild(
            WorkflowPermissions.ServiceWorkflows.Delete,
            L("Permission:Workflow:ServiceWorkflows:Delete")
        );

        var serviceWorkflowStepsPermissions = workflowsGroup.AddPermission(
            WorkflowPermissions.ServiceWorkflowSteps.Default,
            L("Permission:Workflow:ServiceWorkflowSteps")
        );
        serviceWorkflowStepsPermissions.AddChild(
            WorkflowPermissions.ServiceWorkflowSteps.Create,
            L("Permission:Workflow:ServiceWorkflowSteps:Create")
        );
        serviceWorkflowStepsPermissions.AddChild(
            WorkflowPermissions.ServiceWorkflowSteps.Update,
            L("Permission:Workflow:ServiceWorkflowSteps:Update")
        );
        serviceWorkflowStepsPermissions.AddChild(
            WorkflowPermissions.ServiceWorkflowSteps.Delete,
            L("Permission:Workflow:ServiceWorkflowSteps:Delete")
        );

        var serviceWorkflowInstancesPermissions = workflowsGroup.AddPermission(
            WorkflowPermissions.ServiceWorkflowInstances.Default,
            L("Permission:Workflow:ServiceWorkflowInstances")
        );
        serviceWorkflowInstancesPermissions.AddChild(
            WorkflowPermissions.ServiceWorkflowInstances.Create,
            L("Permission:Workflow:ServiceWorkflowInstances:Create")
        );
        serviceWorkflowInstancesPermissions.AddChild(
            WorkflowPermissions.ServiceWorkflowInstances.Update,
            L("Permission:Workflow:ServiceWorkflowInstances:Update")
        );
        serviceWorkflowInstancesPermissions.AddChild(
            WorkflowPermissions.ServiceWorkflowInstances.Delete,
            L("Permission:Workflow:ServiceWorkflowInstances:Delete")
        );

        var serviceWorkflowTasksPermissions = workflowsGroup.AddPermission(
            WorkflowPermissions.ServiceWorkflowTasks.Default,
            L("Permission:Workflow:ServiceWorkflowTasks")
        );
        serviceWorkflowTasksPermissions.AddChild(
            WorkflowPermissions.ServiceWorkflowTasks.Create,
            L("Permission:Workflow:ServiceWorkflowTasks:Create")
        );
        serviceWorkflowTasksPermissions.AddChild(
            WorkflowPermissions.ServiceWorkflowTasks.Update,
            L("Permission:Workflow:ServiceWorkflowTasks:Update")
        );
        serviceWorkflowTasksPermissions.AddChild(
            WorkflowPermissions.ServiceWorkflowTasks.Delete,
            L("Permission:Workflow:ServiceWorkflowTasks:Delete")
        );

        var serviceWorkflowHistoryPermissions = workflowsGroup.AddPermission(
            WorkflowPermissions.ServiceWorkflowHistory.Default,
            L("Permission:Workflow:ServiceWorkflowHistory")
        );
        serviceWorkflowHistoryPermissions.AddChild(
            WorkflowPermissions.ServiceWorkflowHistory.Create,
            L("Permission:Workflow:ServiceWorkflowHistory:Create")
        );
        serviceWorkflowHistoryPermissions.AddChild(
            WorkflowPermissions.ServiceWorkflowHistory.Update,
            L("Permission:Workflow:ServiceWorkflowHistory:Update")
        );
        serviceWorkflowHistoryPermissions.AddChild(
            WorkflowPermissions.ServiceWorkflowHistory.Delete,
            L("Permission:Workflow:ServiceWorkflowHistory:Delete")
        );

        var transcriptionsPermissions = workflowsGroup.AddPermission(
            WorkflowPermissions.Transcriptions.Default,
            L("Permission:Workflow:Transcriptions")
        );
        transcriptionsPermissions.AddChild(
            WorkflowPermissions.Transcriptions.Create,
            L("Permission:Workflow:Transcriptions:Create")
        );
        transcriptionsPermissions.AddChild(
            WorkflowPermissions.Transcriptions.Update,
            L("Permission:Workflow:Transcriptions:Update")
        );
        transcriptionsPermissions.AddChild(
            WorkflowPermissions.Transcriptions.Delete,
            L("Permission:Workflow:Transcriptions:Delete")
        );
        
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<WorkflowResource>(name);
    }
}
