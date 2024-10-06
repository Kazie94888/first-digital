using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Domain.Orders;
using SmartCoinOS.Persistence;
using Spectre.Console;

namespace SmartCoinOS.SmartCoinTool.DataSeed;

internal class DataSeeder
{
    private readonly DataContext _context;
    private readonly bool _shouldResetDb;
    private readonly int _numberOfClients;
    private readonly int _numberOfOrdersPerClient;
    private readonly int _numberOfAuditLogsPerClient;
    private readonly int _numberOfApplicationsPerStatus;
    private readonly int _numberOfFdtDepositAccounts;

    internal DataSeeder(DataContext context, bool shouldResetDb,
        int numberOfClients, int numberOfOrdersPerClient,
        int numberOfAuditLogsPerClient, int numberOfApplicationsPerStatus,
        int numberOfFdtDepositAccounts)
    {
        _context = context;
        _shouldResetDb = shouldResetDb;
        _numberOfClients = numberOfClients;
        _numberOfOrdersPerClient = numberOfOrdersPerClient;
        _numberOfAuditLogsPerClient = numberOfAuditLogsPerClient;
        _numberOfApplicationsPerStatus = numberOfApplicationsPerStatus;
        _numberOfFdtDepositAccounts = numberOfFdtDepositAccounts;
    }

    public async Task EnsureSeedDataAsync()
    {
        if (_shouldResetDb)
            await _context.Database.EnsureDeletedAsync();

        await _context.Database.MigrateAsync();

        Log("Migration completed.");

        await SeedDepositBanksAsync();

        await SeedClientsAsync();

        await SeedAuditLogsAsync();

        await SeedFdtDepositAccountsAsync();

        await SeedDepositWalletsAsync();

        await SeedOrdersAsync();

        await SeedApplicationsAsync();

        Log("Data seed operation completed successfully.");
    }

    private async Task SeedApplicationsAsync()
    {
        var foundApplications = await _context.Applications.AnyAsync();
        if (foundApplications)
        {
            Warn("Applications found in db. No new applications will be generated");
            return;
        }

        var applications = ApplicationSeedProvider.GenerateApplications(_numberOfApplicationsPerStatus);

        var uniqueAppl = (from a in applications
            group a by a.LegalEntityName into applGr
            select applGr.First()).ToList();

        _context.Applications.AddRange(uniqueAppl);

        await _context.SaveChangesAsync();
    }

    private async Task SeedClientsAsync()
    {
        var foundClient = await _context.Clients.AnyAsync();
        if (foundClient)
        {
            Warn("Client data found existing in db. No new clients will be generated.");
            return;
        }

        var knownUsers = await GetKnownUsersAsync();
        var knownUserStack = new Stack<KeyValuePair<string, string>>(knownUsers);
        var clients =
            DataSeedProvider.GenerateClients(_numberOfClients, await _context.DepositBanks.ToListAsync(), knownUserStack);
        _context.Clients.AddRange(clients);

        await _context.SaveChangesAsync();
    }

    private async Task SeedOrdersAsync()
    {
        var foundOrders = await _context.MintOrders.AnyAsync();
        if (foundOrders)
        {
            Warn("Orders found in db. No new orders will be generated");
            return;
        }

        var dbClients = await _context.Clients
            .Include(c => c.Wallets.Where(a => !a.Archived))
            .ToListAsync();

        var defaultDepositWallet = await _context.DepositWallets.FirstAsync(x => x.Default);
        var defaultDepositFdtId = await _context.FdtDepositAccounts.Select(x => x.Id).FirstAsync();

        var fakeOrderNumberGenerator = new FakeOrderNumberGenerator();

        foreach (var client in dbClients)
        {
            var activeWalletIds = client.Wallets.Where(w => !w.Archived).Select(x => x.Id).ToList();
            if (activeWalletIds.Count == 0)
                continue;

            var activeBankAccounts = client.BankAccounts.Where(w => !w.Archived).Select(x => x.Id).ToList();
            if (activeBankAccounts.Count == 0)
                continue;

            var fdtAccountIds = client.FdtAccounts.Select(x => x.Id).ToList();

            var clientOrders = DataSeedProvider.GenerateOrders(fakeOrderNumberGenerator, client.Id, activeWalletIds,
                activeBankAccounts, fdtAccountIds, client.DepositBankId, defaultDepositFdtId, defaultDepositWallet.Id,
                _numberOfOrdersPerClient);

            var orderAuditLogs = DataSeedProvider.GenerateOrderAuditLogs(client.Id, clientOrders, 5);

            foreach (var order in clientOrders)
            {
                if (order is MintOrder mint)
                    _context.MintOrders.Add(mint);

                else if (order is RedeemOrder redeem)
                    _context.RedeemOrders.Add(redeem);
            }

            _context.AuditLogs.AddRange(orderAuditLogs);
        }

        await _context.SaveChangesAsync();
    }

    private async Task SeedAuditLogsAsync()
    {
        var foundAuditLogs = await _context.AuditLogs.AnyAsync();
        if (foundAuditLogs)
        {
            Warn("Audit logs found in db. No new logs will be generated");
            return;
        }

        var dbClients = await _context.Clients.ToListAsync();

        foreach (var client in dbClients)
        {
            var auditLogs = DataSeedProvider.GenerateAuditLogs(client.Id, _numberOfAuditLogsPerClient);
            _context.AuditLogs.AddRange(auditLogs);
        }

        await _context.SaveChangesAsync();
    }

    private async Task SeedDepositBanksAsync()
    {
        var foundDepositBanks = await _context.DepositBanks.AnyAsync();
        if (foundDepositBanks)
        {
            Warn("Deposit banks found in db. No new deposit banks will be generated");
            return;
        }

        var depositBanks = DataSeedProvider.GenerateDepositBanks();
        _context.DepositBanks.AddRange(depositBanks);

        await _context.SaveChangesAsync();
    }

    private async Task SeedDepositWalletsAsync()
    {
        var foundDepositWallets = await _context.DepositWallets.AnyAsync();
        if (foundDepositWallets)
        {
            Warn("Deposit wallets found in db. No new items will be generated");
            return;
        }

        var depositWallets = DataSeedProvider.GenerateDepositWallets();
        _context.DepositWallets.AddRange(depositWallets);

        await _context.SaveChangesAsync();
    }

    private async Task SeedFdtDepositAccountsAsync()
    {
        var foundFdtDepositAccounts = await _context.FdtDepositAccounts.AnyAsync();
        if (foundFdtDepositAccounts)
        {
            Warn("FdtDepositAccounts found in db. No new accounts will be generated");
            return;
        }

        var fdtDepositAccounts = DataSeedProvider.GenerateFdtDepositAccounts(_numberOfFdtDepositAccounts);
        _context.FdtDepositAccounts.AddRange(fdtDepositAccounts);

        await _context.SaveChangesAsync();
    }

    private async Task<Dictionary<string, string>> GetKnownUsersAsync()
    {
        var knownUsers = new Dictionary<string, string>();
        var knownUsersPath = Path.Combine("Data", "KnownDemoUsers.json");
        if (!File.Exists(knownUsersPath))
            Warn(
                $"Known users file was not found at '{knownUsersPath}'. Clients will be generated without connected users.");
        else
        {
            var all = await File.ReadAllTextAsync(knownUsersPath);
            knownUsers = JsonSerializer.Deserialize<Dictionary<string, string>>(all) ?? [];
        }

        return knownUsers;
    }

    private static void Warn(string warning) => AnsiConsole.MarkupLine($"> [yellow]{warning}[/]");

    private static void Log(string info) => AnsiConsole.MarkupLine($"> [lime]{info}[/]");
}
