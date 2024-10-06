using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCoinOS.Domain.Application;
using SmartCoinOS.Persistence.Extensions;
using ApplicationId = SmartCoinOS.Domain.Application.ApplicationId;

namespace SmartCoinOS.Persistence.Configurations;
internal class ApplicationConfiguration : IEntityTypeConfiguration<Application>
{
    public void Configure(EntityTypeBuilder<Application> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
                .HasConversion(x => x.Value, guid => new ApplicationId(guid));

        builder.Property(x => x.LegalEntityName)
                .HasMaxLength(250)
                .IsRequired();

        builder.Property(x => x.ApplicationNumber)
                .HasConversion(appl => appl.Value, val => new ApplicationNumber { Value = val })
                .HasMaxLength(50);

        builder.HasIndex(x => x.ApplicationNumber).IsUnique();

        builder.HasIndex(x => x.LegalEntityName).IsUnique();

        builder.Property(x => x.Status)
                .HasEnumStringConversion();


        builder.Property(x => x.EntityParticulars).HasJsonConversion();
        builder.Property(x => x.Wallets).HasJsonConversion();
        builder.Property(x => x.AuthorizedUsers).HasJsonConversion();
        builder.Property(x => x.BankAccounts).HasJsonConversion();
        builder.Property(x => x.FdtAccounts).HasJsonConversion();
        builder.Property(x => x.BusinessInfo).HasJsonConversion();
        builder.Property(x => x.Documents).HasJsonConversion();
    }
}
