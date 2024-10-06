using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartCoinOS.Persistence.DomainEvents;

namespace SmartCoinOS.Persistence.Extensions;

public static class DependencyInjectionExtensions
{
    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DataContext>((_, options) =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("Default"),
                npgsqlOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorCodesToAdd: null);

                    sqlOptions.MigrationsHistoryTable("__MigrationsHistory");

                    sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });
        });
        services.AddScoped<ReadOnlyDataContext>();
        services.AddScoped<IDomainEventsDispatcher, DomainEventsDispatcher>();
    }
}
