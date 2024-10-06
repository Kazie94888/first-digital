using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.Persistence.Configurations;

internal class OrderDocumentConfiguration : IEntityTypeConfiguration<OrderDocument>
{
    public void Configure(EntityTypeBuilder<OrderDocument> builder)
    {
        builder.ToTable("OrderDocuments");

        builder.HasKey(od => od.Id);
        builder.Property(od => od.Id)
            .HasConversion(id => id.Value, guid => new OrderDocumentId(guid));

        builder.Property(od => od.Key).IsRequired();
        builder.Property(od => od.Name).HasMaxLength(100).IsRequired();
        builder.Property(od => od.ContentType).HasMaxLength(100).IsRequired();
        builder.Property(od => od.DocumentType).HasMaxLength(100).IsRequired();

        builder.Property(od => od.OrderId)
            .HasConversion(id => id.Value, guid => new OrderId(guid));
    }
}