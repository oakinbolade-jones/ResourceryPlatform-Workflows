using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace ResourceryPlatformWorkflow.Documents.EntityFrameworkCore;

[ConnectionStringName(ResourceryPlatformWorkflowNames.DocumentsDb)]
public class DocumentsDbContext(DbContextOptions<DocumentsDbContext> options)
    : AbpDbContext<DocumentsDbContext>(options),
        IDocumentsDbContext
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureDocuments();
    }
}
