using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Persistence.Extensions;

namespace SmartCoinOS.Persistence.Configurations;

internal class EntityParticularConfiguration : IEntityTypeConfiguration<EntityParticular>
{
    public void Configure(EntityTypeBuilder<EntityParticular> builder)
    {
        builder.ToTable("EntityParticulars");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, g => new EntityParticularId(g));

        builder.Property(x => x.VerificationStatus).HasEnumStringConversion();
        builder.Property(x => x.LegalStructure).HasEnumStringConversion();
        builder.Property(x => x.StructureDetails).HasMaxLength(250);

        builder.Property(x => x.LegalEntityName).HasMaxLength(100);
        builder.Property(x => x.RegistrationNumber).HasMaxLength(50);
        builder.Property(x => x.CountryOfInc).HasMaxLength(100);
    }
}