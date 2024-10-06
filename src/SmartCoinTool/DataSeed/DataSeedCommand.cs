using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SmartCoinOS.Persistence;
using Spectre.Console;
using Spectre.Console.Cli;

namespace SmartCoinOS.SmartCoinTool.DataSeed;

internal sealed class DataSeedCommand : AsyncCommand<DataSeedCommand.Settings>
{
    public DataSeedCommand() { }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        await AnsiConsole.Status()
            .StartAsync("DataSeed in progress...", async _ =>
            {
                try
                {
                    var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
                    optionsBuilder.UseNpgsql(settings.GetConnectionString(),
                        npgsqlOptionsAction: sqlOptions =>
                        {
                            sqlOptions.MigrationsHistoryTable("__MigrationsHistory");
                            sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                        });

                    await using var dbContext = new DataContext(optionsBuilder.Options);

                    var dataSeeder = new DataSeeder(dbContext,
                        settings.Reset,
                        settings.NumberOfClients(),
                        settings.NumberOfOrders(),
                        settings.NumberOfAuditLogs(),
                        settings.NumberOfApplications(),
                        settings.NumberOfFdtDepositAccount());

                    await dataSeeder.EnsureSeedDataAsync();
                }
                catch (Exception ex)
                {
                    AnsiConsole.WriteException(ex);
                    throw;
                }
            });

        return 0;
    }

    internal sealed class Settings : CommandSettings
    {
        private const int _defaultNumberOfClients = 50;
        private const int _defaultNumberOfOrders = 10;
        private const int _defaultNumberOfAuditLogs = 50;
        private const int _defaultNumberOfApplications = 3;
        private const int _defaultNumberOfFdtDepositAccounts = 5;

        [CommandOption("--host <Host>")]
        [Description("DB Host. Defaults to `localhost`")]
        public string? Host { get; set; }

        [CommandOption("--port <Port>")]
        [Description("DB Port. Defaults to `5432`")]
        public int? Port { get; set; }

        [CommandOption("--db-name <Database>")]
        [Description("DB Name. Defaults to `smartcoin-os`")]
        public string? Database { get; set; }

        [CommandOption("--username <Username>")]
        [Description("DB Username. Defaults to `postgres`")]
        public string? Username { get; set; }

        [CommandOption("--password <Password>")]
        [Description("DB Password. Defaults to `postgres`")]
        public string? Password { get; set; }

        [CommandOption("--clients <Clients>")]
        [Description("The number of clients to generate. Defaults to `50`")]
        public int? Clients { get; set; }

        [CommandOption("--orders <Orders>")]
        [Description("The number of orders to generate for each client. Defaults to `10`")]
        public int? Orders { get; set; }

        [CommandOption("--auditlogs <AuditLogs>")]
        [Description("The number of audit logs to generate for each client. Defaults to `50`")]
        public int? AuditLogs { get; set; }

        [CommandOption("--applications <Applications>")]
        [Description("The number of applications for each application status to generate. Defaults to `3`")]
        public int? Applications { get; set; }

        [CommandOption("--fdtdepositaccounts <FdtDepositAccount>")]
        [Description("The number of fdt deposit accounts to generate. Defaults to `5`")]
        public int? FdtDepositAccounts { get; set; }

        [CommandOption("--reset")]
        [Description("Drops the db before migrating again.")]
        public bool Reset { get; set; }

        internal string GetConnectionString()
        {
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = Host ?? "localhost",
                Port = Port ?? 5432,
                Database = Database ?? "smartcoin-os",
                Username = Username ?? "postgres",
                Password = Password ?? "postgres",
                IncludeErrorDetail = true
            };

            return connectionStringBuilder.ToString();
        }

        internal int NumberOfClients()
        {
            return Clients ?? _defaultNumberOfClients;
        }

        internal int NumberOfOrders()
        {
            return Orders ?? _defaultNumberOfOrders;
        }

        internal int NumberOfAuditLogs()
        {
            return AuditLogs ?? _defaultNumberOfAuditLogs;
        }

        internal int NumberOfApplications()
        {
            return Applications ?? _defaultNumberOfApplications;
        }

        internal int NumberOfFdtDepositAccount()
        {
            return FdtDepositAccounts ?? _defaultNumberOfFdtDepositAccounts;
        }
    }
}
