using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Persistence.Extensions;

namespace SmartCoinOS.Persistence.Configurations;

internal class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.ToTable("Wallets");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
                .HasConversion(id => id.Value, guid => new WalletId(guid));

        builder.Property(x => x.ClientId)
                .HasConversion(id => id.Value, guid => new ClientId(guid));

        builder.OwnsOne(x => x.WalletAccount, accConfig =>
        {
            accConfig.Property(x => x.Address).IsRequired();
            accConfig.Property(x => x.Network).HasEnumStringConversion().IsRequired();
        });

        builder.Property(x => x.Alias).HasMaxLength(250);

        builder.Property(x => x.VerificationStatus).IsRequired().HasEnumStringConversion();
    }
}