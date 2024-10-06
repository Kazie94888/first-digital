using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Persistence.Extensions;

namespace SmartCoinOS.Persistence.Configurations;

internal class FdtAccountConfiguration : IEntityTypeConfiguration<FdtAccount>
{
    public void Configure(EntityTypeBuilder<FdtAccount> builder)
    {
        builder.ToTable("FdtAccounts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
                .HasConversion(x => x.Value, guid => new FdtAccountId(guid));

        builder.Property(x => x.VerificationStatus)
            .IsRequired()
            .HasEnumStringConversion();

        builder.Property(x => x.ClientId)
                .HasConversion(x => x.Value, guid => new ClientId(guid));

        builder.Property(x => x.ClientName)
                .HasMaxLength(250)
                .IsRequired();

        builder.Property(x => x.AccountNumber)
                .HasConversion(x => x.Value, val => new FdtAccountNumber(val))
                .HasMaxLength(250)
                .IsRequired();

        builder.Property(x => x.Alias)
                .HasMaxLength(250);
    }
}
