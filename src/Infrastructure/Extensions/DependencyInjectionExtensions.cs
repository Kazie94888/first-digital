using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SmartCoinOS.Application.Abstractions.Caching;
using SmartCoinOS.Domain.Application;
using SmartCoinOS.Domain.Orders;
using SmartCoinOS.Domain.SeedWork;
using SmartCoinOS.Infrastructure.Caching;
using SmartCoinOS.Infrastructure.Services;
using SmartCoinOS.Infrastructure.Settings;
using SmartCoinOS.Infrastructure.Settings.Interfaces;

namespace SmartCoinOS.Infrastructure.Extensions;

public static class DependencyInjectionExtensions
{
    public static void AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddMemoryCache();
        builder.Services.AddSingleton<ICacheService, CacheService>();

        builder.Services.AddMediatR(opts =>
        {
            opts.RegisterServicesFromAssemblyContaining<AggregateRoot>();
        });
    }

    public static void AddAppConfig(this IConfigurationBuilder builder)
    {
        var applicationId = Environment.GetEnvironmentVariable("ApplicationId");
        var environmentId = Environment.GetEnvironmentVariable("EnvironmentId");
        var profileId = Environment.GetEnvironmentVariable("ProfileId");
        if (string.IsNullOrWhiteSpace(applicationId) || string.IsNullOrWhiteSpace(environmentId) ||
            string.IsNullOrWhiteSpace(profileId))
            return;
        builder.AddAppConfig(applicationId, environmentId, profileId);
    }

    public static void AddSqlSource(this IConfigurationBuilder builder, string connectionString,
        IServiceCollection services)
    {
        var notifier = new ConfigurationChangedNotifier();
        builder.Add(new PostgresConfigurationSource(connectionString, notifier));
        services.TryAddSingleton<IConfigurationChangedNotifier>(_ => notifier);
    }

    public static void AddDomainServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IOrderNumberService, OrderNumberService>();
        builder.Services.AddTransient<OrderService>();

        builder.Services.AddTransient<IApplicationNumberService, ApplicationNumberService>();
        builder.Services.AddTransient<ApplicationService>();
    }
}