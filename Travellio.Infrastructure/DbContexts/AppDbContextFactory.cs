using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Travellio.Infrastructure.DbContexts;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var apiProjectPath = FindApiProjectPath();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiProjectPath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder
            .UseNpgsql(configuration.GetConnectionString("Sql"))
            .UseSnakeCaseNamingConvention();

        return new AppDbContext(optionsBuilder.Options);
    }

    private static string FindApiProjectPath()
    {
        var current = Directory.GetCurrentDirectory();
        while (current is not null)
        {
            var candidate = Path.Combine(current, "Travellio.Api");
            if (Directory.Exists(candidate))
                return candidate;
            current = Directory.GetParent(current)?.FullName;
        }
        throw new DirectoryNotFoundException("Could not find Travellio.Api directory.");
    }
}