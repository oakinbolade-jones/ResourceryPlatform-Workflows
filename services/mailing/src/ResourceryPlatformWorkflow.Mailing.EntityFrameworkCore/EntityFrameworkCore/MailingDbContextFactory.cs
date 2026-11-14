using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ResourceryPlatformWorkflow.Mailing.EntityFrameworkCore;

public class MailingDbContextFactory : IDesignTimeDbContextFactory<MailingDbContext>
{
    public MailingDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<MailingDbContext>().UseNpgsql(
            GetConnectionStringFromConfiguration()
        );

        return new MailingDbContext(builder.Options);
    }

    private static string GetConnectionStringFromConfiguration()
    {
        return BuildConfiguration().GetConnectionString(MailingDbProperties.ConnectionStringName);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(
                Path.Combine(
                    Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName,
                    $"host{Path.DirectorySeparatorChar}ResourceryPlatformWorkflow.Mailing.HttpApi.Host"
                )
            )
            .AddJsonFile("appsettings.json", false);

        return builder.Build();
    }
}
