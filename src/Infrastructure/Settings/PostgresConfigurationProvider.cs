using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Npgsql;
using SmartCoinOS.Infrastructure.Settings.Interfaces;

namespace SmartCoinOS.Infrastructure.Settings;

public sealed class PostgresConfigurationProvider : ConfigurationProvider
{
    private readonly string _connectionString;
    private bool _isTableCreated;
    private const string _schema = "settings";
    private const string _table = "Settings";

    public PostgresConfigurationProvider(string connectionString, IConfigurationChangedNotifier notifier)
    {
        _connectionString = connectionString;
        ChangeToken.OnChange(notifier.GetReloadToken, Load);
    }
    public override void Load()
    {
        if (!_isTableCreated)
        {
            CreateTableIfNotExists();
            _isTableCreated = true;
        }
        const string sql = $"""SELECT "Key", "Value" FROM {_schema}."{_table}";""";
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        using var command = new NpgsqlCommand(sql, connection);
        using var reader = command.ExecuteReader();
        var data = new Dictionary<string, string?>();
        while (reader.Read())
        {
            var key = reader.GetString(0);
            var value = reader.GetString(1);
            data[key] = value;
        }
        Data = data;
    }

    private void CreateTableIfNotExists()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        using var command = new NpgsqlCommand($"CREATE SCHEMA IF NOT EXISTS {_schema};", connection);
        command.ExecuteNonQuery();
        command.CommandText = $"CREATE TABLE IF NOT EXISTS {_schema}.\"{_table}\"(\"Key\" text PRIMARY KEY, \"Value\" text, \"DisplayName\" text NOT NULL);";
        command.ExecuteNonQuery();
    }
}