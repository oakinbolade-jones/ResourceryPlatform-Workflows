using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace ResourceryPlatformWorkflow.Mailing.EntityFrameworkCore;

[ConnectionStringName(ResourceryPlatformWorkflowNames.MailingDb)]
public class MailingDbContext(DbContextOptions<MailingDbContext> options)
    : AbpDbContext<MailingDbContext>(options),
        IMailingDbContext
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureMailing();
    }
}
