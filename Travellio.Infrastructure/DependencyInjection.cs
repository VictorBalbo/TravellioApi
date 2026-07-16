using Amazon.Runtime;
using Amazon.S3;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using StackExchange.Redis;
using Travellio.Infrastructure.DbContexts;

namespace Travellio.Infrastructure;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddDatabaseInfrastructure(IConfiguration configuration)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            var sqlConnectionString = configuration.GetConnectionString("Sql");
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(sqlConnectionString).UseSnakeCaseNamingConvention());

            return services;
        }

        public IServiceCollection AddRedisInfrastructure(IConfiguration configuration)
        {
            var redisConnectionString = configuration.GetConnectionString("Redis");
            if (!string.IsNullOrEmpty(redisConnectionString))
            {
                services.AddSingleton<IConnectionMultiplexer>(_ =>
                    ConnectionMultiplexer.Connect(redisConnectionString));
            }

            return services;
        }

        public IServiceCollection AddR2Infrastructure(IConfiguration configuration)
        {
            var accountId = configuration["R2:AccountId"];
            var accessKeyId = configuration["R2:AccessKeyId"];
            var secretAccessKey = configuration["R2:SecretAccessKey"];
            var bucketName = configuration["R2:BucketName"];
            var publicUrl = configuration["R2:PublicUrl"];

            if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(accessKeyId) ||
                string.IsNullOrEmpty(secretAccessKey) || string.IsNullOrEmpty(bucketName) ||
                string.IsNullOrEmpty(publicUrl))
            {
                throw new InvalidOperationException(
                    "R2 storage is not configured. Set R2:AccountId, R2:AccessKeyId, R2:SecretAccessKey, R2:BucketName and R2:PublicUrl.");
            }

            services.AddSingleton<IAmazonS3>(_ => new AmazonS3Client(
                new BasicAWSCredentials(accessKeyId, secretAccessKey),
                new AmazonS3Config
                {
                    ServiceURL = $"https://{accountId}.eu.r2.cloudflarestorage.com",
                    ForcePathStyle = true,
                    AuthenticationRegion = "auto",
                    // R2 doesn't support the SDK's default streaming checksum trailers.
                    RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED,
                    ResponseChecksumValidation = ResponseChecksumValidation.WHEN_REQUIRED
                }));

            return services;
        }
    }

    extension(IHostBuilder host)
    {
        public IHostBuilder AddSerilog()
        {
            host.UseSerilog((context, config) =>
                config.ReadFrom.Configuration(context.Configuration));

            return host;
        }
    }

    extension(WebApplication app)
    {
        public IApplicationBuilder AddRequestLogging()
        {
            return app.UseSerilogRequestLogging(options =>
            {
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    var endpoint = httpContext.GetEndpoint() as RouteEndpoint;
                    var routePattern = endpoint?.RoutePattern.RawText ?? httpContext.Request.Path;

                    diagnosticContext.Set("RouteTemplate", routePattern);
                };
            });
        }
}

}