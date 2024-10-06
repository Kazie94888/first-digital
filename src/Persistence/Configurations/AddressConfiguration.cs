using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Shared;

namespace SmartCoinOS.Persistence.Configurations;

internal class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("Addresses");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
                .HasConversion(x => x.Value, guid => new AddressId(guid));

        builder.HasOne<Client>()
            .WithOne(c => c.Address)
            .HasForeignKey<Client>("AddressId")
            .IsRequired();

        builder.HasOne<BankAccount>()
            .WithOne(b => b.Address)
            .HasForeignKey<BankAccount>("AddressId")
            .IsRequired();

        builder.Property(c => c.Country).HasMaxLength(100);
        builder.Property(s => s.State).HasMaxLength(250);
        builder.Property(x => x.PostalCode).HasMaxLength(15);
        builder.Property(x => x.City).HasMaxLength(250);
        builder.Property(x => x.Street).HasMaxLength(250);
    }
}