using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SmartCoinOS.Persistence;

public sealed class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        var connectionString = GetConnectionString();

        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
        optionsBuilder.UseNpgsql(connectionString,
            npgsqlOptionsAction: sqlOptions =>
            {
                sqlOptions.MigrationsHistoryTable("__MigrationsHistory");
                sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });


        return new DataContext(optionsBuilder.Options);
    }

    private static string GetConnectionString()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{env}.json", true);

        builder.AddEnvironmentVariables();

        IConfiguration config = builder.Build();

        var connectionString = config.GetConnectionString("MigrationContextConnection");
        if (connectionString is null)
            throw new InvalidOperationException("Unable to find connection string for migration");
        return connectionString;
    }
}
