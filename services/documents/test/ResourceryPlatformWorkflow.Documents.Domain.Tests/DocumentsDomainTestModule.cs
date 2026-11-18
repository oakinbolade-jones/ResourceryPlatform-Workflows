using ResourceryPlatformWorkflow.Documents.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace ResourceryPlatformWorkflow.Documents;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(typeof(DocumentsEntityFrameworkCoreTestModule))]
public class DocumentsDomainTestModule : AbpModule { }
