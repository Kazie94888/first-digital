using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Domain.Application;
using SmartCoinOS.Domain.AuditLogs;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Deposit;
using SmartCoinOS.Domain.Orders;
using SmartCoinOS.Domain.Shared;
using SmartCoinOS.Persistence.Entities;
using SmartCoinOS.Persistence.Extensions;

namespace SmartCoinOS.Persistence;

public sealed class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<ApplicationSettings> Settings => Set<ApplicationSettings>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Application> Applications => Set<Application>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DepositBank> DepositBanks => Set<DepositBank>();
    public DbSet<DepositWallet> DepositWallets => Set<DepositWallet>();
    public DbSet<DepositFdtAccount> FdtDepositAccounts => Set<DepositFdtAccount>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<MintOrder> MintOrders => Set<MintOrder>();
    public DbSet<RedeemOrder> RedeemOrders => Set<RedeemOrder>();


    internal DbSet<OrderDocument> OrderDocuments => Set<OrderDocument>();
    internal DbSet<Address> Addresses => Set<Address>();
    internal DbSet<EntityParticular> EntityParticulars => Set<EntityParticular>();
    internal DbSet<AuthorizedUser> AuthorizedUsers => Set<AuthorizedUser>();
    internal DbSet<BankAccount> BankAccounts => Set<BankAccount>();
    internal DbSet<Wallet> Wallets => Set<Wallet>();
    internal DbSet<FdtAccount> FdtAccounts => Set<FdtAccount>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
        modelBuilder.ApplyBaseEntityProperties();
        modelBuilder.ApplyEntityVersioning();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return SaveChangesAsync(acceptAllChangesOnSuccess: true, cancellationToken);
    }

    public override int SaveChanges() => throw new InvalidOperationException();

    public override int SaveChanges(bool acceptAllChangesOnSuccess) => throw new InvalidOperationException();
}
