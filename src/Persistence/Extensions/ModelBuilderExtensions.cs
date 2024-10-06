using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Domain.Exceptions;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Persistence.Extensions;

internal static class ModelBuilderExtensions
{
    internal static void ApplyBaseEntityProperties(this ModelBuilder builder)
    {
        var mutableEntityTypes = builder.Model.GetEntityTypes()
            .Where(x => typeof(Entity).IsAssignableFrom(x.ClrType))
            .ToList();

        var serializer = new JsonSerializerOptions();
        foreach (var type in mutableEntityTypes.Select(entity => entity.ClrType))
        {
            builder.Entity(type).Property<DateTimeOffset>(nameof(Entity.CreatedAt));

            builder.Entity(type).Property<UserInfo>(nameof(Entity.CreatedBy))
                .HasColumnType("jsonb")
                .HasConversion(
                    userInfo => JsonSerializer.Serialize(userInfo, serializer),
                    userInfoJsonString => JsonSerializer.Deserialize<UserInfo>(userInfoJsonString, serializer)
                                          ?? NullEntityException<UserInfo>.Throw()
                );
        }
    }

    internal static void ApplyEntityVersioning(this ModelBuilder builder)
    {
        var aggregates = builder.Model.GetEntityTypes()
                            .Where(x => typeof(AggregateRoot).IsAssignableFrom(x.ClrType))
                            .ToList();

        foreach (var aggregate in aggregates)
        {
            builder.Entity(aggregate.ClrType).Property<Guid>(nameof(AggregateRoot.CurrentVersion)).IsConcurrencyToken();
        }
    }
}