using Microsoft.Extensions.Hosting;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace ResourceryPlatformWorkflow.Mailing.EntityFrameworkCore;

[ConnectionStringName(ResourceryPlatformWorkflowNames.MailingDb)]
public interface IMailingDbContext : IEfCoreDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * DbSet<Question> Questions { get; }
     */
}
