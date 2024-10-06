using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCoinOS.Domain.AuditLogs;

namespace SmartCoinOS.Persistence.Configurations;

internal sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, guid => new AuditLogId(guid));

        builder.OwnsMany(a => a.Parameters, onb =>
        {
            onb.ToJson();
        });

        builder.Property(x => x.Event).HasMaxLength(100);
    }
}