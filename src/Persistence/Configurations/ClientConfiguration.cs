using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Deposit;
using SmartCoinOS.Persistence.Extensions;

namespace SmartCoinOS.Persistence.Configurations;

internal class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    private readonly JsonSerializerOptions _serializer = new();
    private readonly IReadOnlyList<ClientDocument> _defaultDocuments = [];

    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Clients");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, g => new ClientId(g));

        builder.Property(x => x.Status)
            .HasEnumStringConversion();

        builder.HasMany(x => x.EntityParticulars)
            .WithOne()
            .HasForeignKey(c => c.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Wallets)
            .WithOne()
            .HasForeignKey(c => c.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.BankAccounts)
            .WithOne()
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.AuthorizedUsers)
            .WithOne()
            .HasForeignKey(x => x.ClientId);

        builder.OwnsOne(x => x.Contact, contact =>
        {
            contact.Property(c => c.Email).HasColumnName("ContactEmail");
            contact.Property(c => c.Phone).HasColumnName("ContactPhone");
        });

        builder.Property(x => x.Documents)
            .HasColumnType("jsonb")
            .HasConversion(
                    obj => JsonSerializer.Serialize(obj, _serializer),
                    str => string.IsNullOrEmpty(str) ? _defaultDocuments : JsonSerializer.Deserialize<List<ClientDocument>>(str, _serializer)!.ToList(),
                    ValueComparers<ClientDocument>.GetCollectionComparerByValue()
            );

        builder.HasMany(f => f.FdtAccounts)
               .WithOne()
               .HasForeignKey(fk => fk.ClientId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.DepositBankId).HasConversion(
                    new ValueConverter<DepositBankId?, Guid?>(
                    toStore => toStore == null ? null : toStore.Value.Value,
                    toObject => toObject == null ? null : new DepositBankId(toObject.Value)));

        builder.Navigation(x => x.Address).AutoInclude();
        builder.Navigation(x => x.EntityParticulars).AutoInclude();
        builder.Navigation(x => x.Wallets).AutoInclude();
        builder.Navigation(x => x.AuthorizedUsers).AutoInclude();
        builder.Navigation(x => x.BankAccounts).AutoInclude();
        builder.Navigation(x => x.FdtAccounts).AutoInclude();
    }
}