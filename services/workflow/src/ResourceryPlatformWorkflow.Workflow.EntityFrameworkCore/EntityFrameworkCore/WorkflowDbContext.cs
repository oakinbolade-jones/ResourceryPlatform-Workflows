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
public class WorkflowDbContext(DbContextOptions<WorkflowDbContext> options)
    : AbpDbContext<WorkflowDbContext>(options),
        IWorkflowDbContext
{
    public DbSet<Request> Requests { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<ServiceCenter> ServiceCenters { get; set; }
    public DbSet<ServiceWorkflow> ServiceWorkflows { get; set; }
    public DbSet<ServiceWorkflowStep> ServiceWorkflowSteps { get; set; }
    public DbSet<ServiceWorkflowInstance> ServiceWorkflowInstances { get; set; }
    public DbSet<ServiceWorkflowTask> ServiceWorkflowTasks { get; set; }
    public DbSet<ServiceWorkflowHistory> ServiceWorkflowHistoryEntries { get; set; }
    public DbSet<Meeting> Meetings { get; set; }
    public DbSet<MeetingRequirement> MeetingRequirements { get; set; }
    public DbSet<MeetingItem> MeetingItems { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

         builder.ConfigureWorkflow();
    }
}
