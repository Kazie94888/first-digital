using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCoinOS.Domain.Deposit;

namespace SmartCoinOS.Persistence.Configurations;
internal class DepositBankConfiguration : IEntityTypeConfiguration<DepositBank>
{
    public void Configure(EntityTypeBuilder<DepositBank> builder)
    {
        builder.ToTable("DepositBanks");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, guid => new DepositBankId(guid));

        builder.Property(x => x.Beneficiary).HasMaxLength(100);
        builder.Property(x => x.Iban).HasMaxLength(34);
        builder.Property(x => x.Name).HasMaxLength(250);
        builder.Property(x => x.Swift).HasMaxLength(11);

        builder.Navigation(x => x.Address).AutoInclude();
    }
}
