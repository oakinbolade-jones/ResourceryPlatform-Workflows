using Microsoft.Extensions.Hosting;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace ResourceryPlatformWorkflow.Documents.EntityFrameworkCore;

[ConnectionStringName(ResourceryPlatformWorkflowNames.DocumentsDb)]
public interface IDocumentsDbContext : IEfCoreDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * DbSet<Question> Questions { get; }
     */
}
