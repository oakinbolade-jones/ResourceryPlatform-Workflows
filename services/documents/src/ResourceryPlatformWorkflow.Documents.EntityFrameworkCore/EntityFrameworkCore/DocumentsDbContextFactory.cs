using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ResourceryPlatformWorkflow.Documents.EntityFrameworkCore;

public class DocumentsDbContextFactory : IDesignTimeDbContextFactory<DocumentsDbContext>
{
    public DocumentsDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<DocumentsDbContext>().UseNpgsql(
            GetConnectionStringFromConfiguration()
        );

        return new DocumentsDbContext(builder.Options);
    }

    private static string GetConnectionStringFromConfiguration()
    {
        return BuildConfiguration().GetConnectionString(DocumentsDbProperties.ConnectionStringName);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(
                Path.Combine(
                    Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName,
                    $"host{Path.DirectorySeparatorChar}ResourceryPlatformWorkflow.Documents.HttpApi.Host"
                )
            )
            .AddJsonFile("appsettings.json", false);

        return builder.Build();
    }
}
