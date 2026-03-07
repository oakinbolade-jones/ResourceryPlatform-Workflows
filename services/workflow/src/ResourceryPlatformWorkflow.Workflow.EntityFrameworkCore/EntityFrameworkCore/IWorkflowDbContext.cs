using Microsoft.Extensions.Hosting;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace ResourceryPlatformWorkflow.Workflow.EntityFrameworkCore;

[ConnectionStringName(ResourceryPlatformWorkflowNames.WorkflowDb)]
public interface IWorkflowDbContext : IEfCoreDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * DbSet<Question> Questions { get; }
     */
}
