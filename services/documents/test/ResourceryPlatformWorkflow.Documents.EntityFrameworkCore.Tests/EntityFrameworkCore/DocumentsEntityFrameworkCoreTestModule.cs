using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;

namespace ResourceryPlatformWorkflow.Documents.EntityFrameworkCore;

[DependsOn(typeof(DocumentsTestBaseModule))]
[DependsOn(typeof(DocumentsEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpEntityFrameworkCoreSqliteModule))]
public class DocumentsEntityFrameworkCoreTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var sqliteConnection = CreateDatabaseAndGetConnection();

        Configure<AbpDbContextOptions>(options =>
        {
            options.Configure(abpDbContextConfigurationContext =>
            {
                abpDbContextConfigurationContext.DbContextOptions.UseSqlite(sqliteConnection);
            });
        });

        context.Services.AddAlwaysDisableUnitOfWorkTransaction();
    }

    private static SqliteConnection CreateDatabaseAndGetConnection()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        new DocumentsDbContext(
            new DbContextOptionsBuilder<DocumentsDbContext>().UseSqlite(connection).Options
        )
            .GetService<IRelationalDatabaseCreator>()
            .CreateTables();

        return connection;
    }
}
