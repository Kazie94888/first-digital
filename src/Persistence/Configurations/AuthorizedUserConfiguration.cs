using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Persistence.Extensions;

namespace SmartCoinOS.Persistence.Configurations;

internal sealed class AuthorizedUserConfiguration : IEntityTypeConfiguration<AuthorizedUser>
{
    public void Configure(EntityTypeBuilder<AuthorizedUser> builder)
    {
        builder.ToTable("AuthorizedUsers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
                .HasConversion(id => id.Value, guid => new AuthorizedUserId(guid));

        builder.Property(x => x.FirstName).HasMaxLength(50);
        builder.Property(x => x.LastName).HasMaxLength(50);
        builder.Property(x => x.Email).HasMaxLength(100);
        builder.Property(x => x.ExternalId).HasMaxLength(50);
        builder.Property(x => x.Status).HasEnumStringConversion();

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.Property(x => x.ClientId)
            .HasConversion(id => id.Value, guid => new ClientId(guid));
    }
}