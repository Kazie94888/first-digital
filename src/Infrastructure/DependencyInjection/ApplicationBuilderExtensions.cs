using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using SmartCoinOS.Infrastructure.Extensions;

namespace SmartCoinOS.Infrastructure.DependencyInjection;

public static class ApplicationBuilderExtensions
{
    public static void AddMonitoring(this WebApplicationBuilder builder)
    {
        var tracingSection = builder.Configuration.GetSection("Tracing");

        if (tracingSection.Exists() && tracingSection.GetValue("Enabled", false))
            builder.Services.AddTracing(opts => builder.Configuration.Bind("Tracing", opts),
                builder.Environment.ApplicationName);
    }

    public static void AddConfiguration(this WebApplicationBuilder builder)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory());
        configBuilder.AddJsonFile("appsettings.json");
        configBuilder.AddJsonFile($"appsettings.{environmentName}.json");

        configBuilder.AddAppConfig();

        // Build to get connection string for SQL Provider from first sources
        var config = configBuilder.Build();

        var settingsConnectionString = config.GetConnectionString("Default");
        if (settingsConnectionString is not null)
            configBuilder.AddSqlSource(settingsConnectionString, builder.Services);
        configBuilder.AddEnvironmentVariables();
        builder.Configuration.AddConfiguration(configBuilder.Build());
    }
}