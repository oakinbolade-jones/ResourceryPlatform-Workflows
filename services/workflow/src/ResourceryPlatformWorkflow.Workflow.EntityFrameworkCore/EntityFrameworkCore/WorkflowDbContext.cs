using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace ResourceryPlatformWorkflow.Workflow.EntityFrameworkCore;

[ConnectionStringName(ResourceryPlatformWorkflowNames.WorkflowDb)]
public class WorkflowDbContext(DbContextOptions<WorkflowDbContext> options)
    : AbpDbContext<WorkflowDbContext>(options),
        IWorkflowDbContext
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureWorkflow();
    }
}
