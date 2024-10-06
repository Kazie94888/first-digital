using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Persistence.Extensions;

namespace SmartCoinOS.Persistence.Configurations;
internal class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
{
    public void Configure(EntityTypeBuilder<BankAccount> builder)
    {
        builder.ToTable("BankAccounts");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, guid => new BankAccountId(guid));

        builder.Property(x => x.ClientId)
            .HasConversion(x => x.Value, guid => new ClientId(guid));

        builder.Property(x => x.Beneficiary).HasMaxLength(100);
        builder.Property(x => x.Iban).HasMaxLength(34);
        builder.Property(x => x.BankName).HasMaxLength(250);
        builder.Property(x => x.SwiftCode).HasMaxLength(11);
        builder.Property(x => x.Alias).HasMaxLength(100);
        builder.Property(x => x.VerificationStatus).IsRequired().HasEnumStringConversion();
        builder.Property(x => x.SmartTrustBank).HasJsonConversion();

        builder.Navigation(x => x.Address).AutoInclude();
    }
}
