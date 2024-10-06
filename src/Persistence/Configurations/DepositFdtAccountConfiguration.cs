using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCoinOS.Domain.Deposit;

namespace SmartCoinOS.Persistence.Configurations;
internal class DepositFdtAccountConfiguration : IEntityTypeConfiguration<DepositFdtAccount>
{
    public void Configure(EntityTypeBuilder<DepositFdtAccount> builder)
    {
        builder.ToTable("DepositFdtAccounts");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, guid => new DepositFdtAccountId(guid));

        builder.Property(x => x.AccountName)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.AccountNumber)
            .HasConversion(x => x.Value, val => new FdtDepositAccountNumber(val))
            .HasMaxLength(250)
            .IsRequired();
    }
}
