using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Shared;

namespace SmartCoinOS.Persistence.Configurations;

internal sealed class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("Documents");

        builder.HasKey(cd => cd.Id);
        builder.Property(cd => cd.Id)
            .HasConversion(id => id.Value, guid => new DocumentId(guid));

        builder.Property(o => o.ApplicationId).HasConversion(
            new ValueConverter<Domain.Application.ApplicationId?, Guid?>(
            toStore => toStore == null ? null : toStore.Value.Value,
            toObject => toObject == null ? null : new SmartCoinOS.Domain.Application.ApplicationId(toObject.Value)));

        builder.Property(x => x.ClientId).HasConversion(
            new ValueConverter<ClientId?, Guid?>(
            toStore => toStore == null ? null : toStore.Value.Value,
            toObject => toObject == null ? null : new ClientId(toObject.Value)));

        builder.Property(cd => cd.Key).IsRequired();
        builder.Property(od => od.FileName).HasMaxLength(255).IsRequired();

        builder.Property(cd => cd.ApplicationDocumentType).HasMaxLength(250);

        builder.Property(cd => cd.ContentType).HasMaxLength(100).IsRequired();
    }
}