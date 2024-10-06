using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Domain.Application;
using SmartCoinOS.Domain.AuditLogs;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Deposit;
using SmartCoinOS.Domain.Orders;
using SmartCoinOS.Domain.Shared;

namespace SmartCoinOS.Persistence;

public sealed class ReadOnlyDataContext
{
    private readonly DataContext _context;

    public ReadOnlyDataContext(DataContext context)
    {
        _context = context;
        _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public IQueryable<Order> Orders => _context.Orders.IgnoreAutoIncludes();
    public IQueryable<OrderDocument> OrderDocuments => _context.OrderDocuments.IgnoreAutoIncludes();
    public IQueryable<AuditLog> AuditLogs => _context.AuditLogs.IgnoreAutoIncludes();
    public IQueryable<Address> Addresses => _context.Addresses.IgnoreAutoIncludes();
    public IQueryable<EntityParticular> EntityParticulars => _context.EntityParticulars.IgnoreAutoIncludes();
    public IQueryable<Client> Clients => _context.Clients.IgnoreAutoIncludes();
    public IQueryable<AuthorizedUser> AuthorizedUsers => _context.AuthorizedUsers.IgnoreAutoIncludes();
    public IQueryable<BankAccount> BankAccounts => _context.BankAccounts.IgnoreAutoIncludes();
    public IQueryable<Wallet> Wallets => _context.Wallets.IgnoreAutoIncludes();
    public IQueryable<FdtAccount> FdtAccounts => _context.FdtAccounts.IgnoreAutoIncludes();
    public IQueryable<Application> Applications => _context.Applications.IgnoreAutoIncludes();
    public IQueryable<Document> Documents => _context.Documents.IgnoreAutoIncludes();
    public IQueryable<DepositBank> DepositBanks => _context.DepositBanks.IgnoreAutoIncludes();
    public IQueryable<DepositWallet> DepositWallets => _context.DepositWallets.IgnoreAutoIncludes();
    public IQueryable<DepositFdtAccount> FdtDepositAccounts => _context.FdtDepositAccounts.IgnoreAutoIncludes();
}