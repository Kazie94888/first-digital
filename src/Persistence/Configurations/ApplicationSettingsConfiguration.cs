using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCoinOS.Persistence.Entities;

namespace SmartCoinOS.Persistence.Configurations;
internal class ApplicationSettingsConfiguration : IEntityTypeConfiguration<ApplicationSettings>
{
    public void Configure(EntityTypeBuilder<ApplicationSettings> builder)
    {
        builder.ToTable("Settings", "settings", x => x.ExcludeFromMigrations());
        builder.HasKey(x => x.Key);
        builder.Property(x => x.DisplayName).IsRequired();
    }
}