using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Deposit;
using SmartCoinOS.Domain.Orders;
using SmartCoinOS.Persistence.Extensions;

namespace SmartCoinOS.Persistence.Configurations;

internal class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasDiscriminator(x => x.Type)
            .HasValue<MintOrder>(OrderType.Mint)
            .HasValue<RedeemOrder>(OrderType.Redeem);

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id).HasConversion(id => id.Value, guid => new OrderId(guid));

        builder.Property(o => o.OrderNumber).HasJsonConversion().IsRequired();
        
        builder.Property(x => x.RsnReference).HasJsonConversion();
        
        builder.Property(o => o.ClientId).HasConversion(x => x.Value, g => new ClientId(g)).IsRequired();

        builder.Property(x => x.SafeTxHash).HasMaxLength(120);

        builder.Property(x => x.RedeemTxHash).HasMaxLength(120);

        builder.Property(x => x.Signatures).HasJsonConversion();

        builder.Property(o => o.Type).HasEnumStringConversion();

        builder.Property(o => o.Status).HasEnumStringConversion();

        builder.Property(o => o.ProcessingStatus).HasEnumStringConversion();

        ConfigureNullableReferences(builder);

        ConfigureOwnerships(builder);

        builder.HasMany(o => o.Documents)
            .WithOne()
            .HasForeignKey(doc => doc.OrderId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<BankAccount>()
            .WithMany()
            .HasForeignKey(o => o.BankAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Wallet>()
            .WithMany()
            .HasForeignKey(o => o.WalletId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<FdtAccount>()
            .WithMany()
            .HasForeignKey(o => o.FdtAccountId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.HasIndex(x => x.OrderNumber).IsUnique();
        builder.Navigation(x => x.Documents).AutoInclude();
    }

    private static void ConfigureOwnerships(EntityTypeBuilder<Order> builder)
    {
        builder.OwnsOne(o => o.OrderedAmount, money =>
        {
            money.Property(m => m.Amount).HasColumnName("OrderedAmount");
            money.Property(m => m.Currency).HasColumnName("OrderedCurrency");
        });

        builder.OwnsOne(o => o.DepositedAmount, money =>
        {
            money.Property(m => m.Amount).HasColumnName("DepositAmount");
            money.Property(m => m.Currency).HasColumnName("DepositCurrency");
        });

        builder.OwnsOne(o => o.ActualAmount, money =>
        {
            money.Property(m => m.Amount).HasColumnName("ActualAmount");
            money.Property(m => m.Currency).HasColumnName("ActualCurrency");
        });

        builder.OwnsOne(d => d.DepositInstruction).WithOwner();
        builder.OwnsOne(d => d.PaymentInstruction).WithOwner();
    }

    private static void ConfigureNullableReferences(EntityTypeBuilder<Order> builder)
    {
        builder.Property(o => o.BankAccountId).HasConversion(
            new ValueConverter<BankAccountId?, Guid?>(
                toStore => toStore == null ? null : toStore.Value.Value,
                toObject => toObject == null ? null : new BankAccountId(toObject.Value)));

        builder.Property(x => x.FdtAccountId).HasConversion(
            new ValueConverter<FdtAccountId?, Guid?>(
                toStore => toStore == null ? null : toStore.Value.Value,
                toObject => toObject == null ? null : new FdtAccountId(toObject.Value)));

        builder.Property(x => x.DepositBankId).HasConversion(
            new ValueConverter<DepositBankId?, Guid?>(
                toStore => toStore == null ? null : toStore.Value.Value,
                toObject => toObject == null ? null : new DepositBankId(toObject.Value)));

        builder.Property(x => x.DepositFdtAccountId).HasConversion(
            new ValueConverter<DepositFdtAccountId?, Guid?>(
                toStore => toStore == null ? null : toStore.Value.Value,
                toObject => toObject == null ? null : new DepositFdtAccountId(toObject.Value)));

        builder.Property(x => x.DepositWalletId).HasConversion(
            new ValueConverter<DepositWalletId?, Guid?>(
                toStore => toStore == null ? null : toStore.Value.Value,
                toObject => toObject == null ? null : new DepositWalletId(toObject.Value)));
    }
}
