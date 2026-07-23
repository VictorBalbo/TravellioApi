using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using Travellio.Api.Services.PlaceProviders;
using Travellio.Infrastructure.DbContexts;
using Travellio.IntegrationTests.Mocks;

namespace Travellio.IntegrationTests;

public sealed class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:18")
        .WithDatabase("travellio_test")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    static ApiFactory()
    {
        // Program.cs requires R2 config at startup even though no integration test touches image upload.
        Environment.SetEnvironmentVariable("R2__AccountId", "test-account");
        Environment.SetEnvironmentVariable("R2__AccessKeyId", "test-access-key");
        Environment.SetEnvironmentVariable("R2__SecretAccessKey", "test-secret-key");
        Environment.SetEnvironmentVariable("R2__BucketName", "test-bucket");
        Environment.SetEnvironmentVariable("R2__PublicUrl", "https://example.com");
    }

    public async ValueTask InitializeAsync()
    {
        await _postgres.StartAsync();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.AddDbContext<AppDbContext>(options =>
                options
                    .UseNpgsql(_postgres.GetConnectionString())
                    .UseSnakeCaseNamingConvention());

            services.RemoveAll<IPlaceProvider>();
            services.AddScoped<IPlaceProvider, FakeWanderlogProvider>();
        });
    }

    public new async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
        await base.DisposeAsync();
    }
}