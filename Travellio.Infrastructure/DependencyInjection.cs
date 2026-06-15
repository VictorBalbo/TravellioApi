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