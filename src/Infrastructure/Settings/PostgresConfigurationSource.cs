using Microsoft.Extensions.Configuration;
using SmartCoinOS.Infrastructure.Settings.Interfaces;

namespace SmartCoinOS.Infrastructure.Settings;

public sealed class PostgresConfigurationSource : IConfigurationSource
{
    private readonly string _connectionString;
    private readonly IConfigurationChangedNotifier _notifier;

    public PostgresConfigurationSource(string connectionString, IConfigurationChangedNotifier notifier)
    {
        _connectionString = connectionString;
        _notifier = notifier;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new PostgresConfigurationProvider(_connectionString, _notifier);
    }
}