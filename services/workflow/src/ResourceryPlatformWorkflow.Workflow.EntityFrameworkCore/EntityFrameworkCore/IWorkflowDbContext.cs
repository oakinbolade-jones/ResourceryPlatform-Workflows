using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using ResourceryPlatformWorkflow.Workflow.Meetings;
using ResourceryPlatformWorkflow.Workflow.Requests;
using ResourceryPlatformWorkflow.Workflow.Services;
using ResourceryPlatformWorkflow.Workflow.ServiceWorkflows;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace ResourceryPlatformWorkflow.Workflow.EntityFrameworkCore;

[ConnectionStringName(ResourceryPlatformWorkflowNames.WorkflowDb)]
public interface IWorkflowDbContext : IEfCoreDbContext
{
    DbSet<Request> Requests { get; }
    DbSet<Service> Services { get; }
    DbSet<ServiceCenter> ServiceCenters { get; }
    DbSet<ServiceWorkflow> ServiceWorkflows { get; }
    DbSet<ServiceWorkflowStep> ServiceWorkflowSteps { get; }
    DbSet<ServiceWorkflowInstance> ServiceWorkflowInstances { get; }
    DbSet<ServiceWorkflowTask> ServiceWorkflowTasks { get; }
    DbSet<ServiceWorkflowHistory> ServiceWorkflowHistoryEntries { get; }
    DbSet<Meeting> Meetings { get;  }
    DbSet<MeetingRequirement> MeetingRequirements { get; }
    DbSet<MeetingItem> MeetingItems { get;  }
}
