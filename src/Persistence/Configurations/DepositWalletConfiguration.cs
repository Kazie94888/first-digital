using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCoinOS.Domain.Deposit;
using SmartCoinOS.Persistence.Extensions;

namespace SmartCoinOS.Persistence.Configurations;

internal class DepositWalletConfiguration : IEntityTypeConfiguration<DepositWallet>
{
    public void Configure(EntityTypeBuilder<DepositWallet> builder)
    {
        builder.ToTable("DepositWallets");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
                .HasConversion(id => id.Value, guid => new DepositWalletId(guid));

        builder.OwnsOne(x => x.Account, accConfig =>
        {
            accConfig.Property(x => x.Address).IsRequired();
            accConfig.Property(x => x.Network).HasEnumStringConversion().IsRequired();
        });
    }
}