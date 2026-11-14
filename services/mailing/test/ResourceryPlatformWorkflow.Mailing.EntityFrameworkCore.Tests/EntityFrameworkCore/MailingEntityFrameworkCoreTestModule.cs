using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;

namespace ResourceryPlatformWorkflow.Mailing.EntityFrameworkCore;

[DependsOn(typeof(MailingTestBaseModule))]
[DependsOn(typeof(MailingEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpEntityFrameworkCoreSqliteModule))]
public class MailingEntityFrameworkCoreTestModule : AbpModule
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

        new MailingDbContext(
            new DbContextOptionsBuilder<MailingDbContext>().UseSqlite(connection).Options
        )
            .GetService<IRelationalDatabaseCreator>()
            .CreateTables();

        return connection;
    }
}
