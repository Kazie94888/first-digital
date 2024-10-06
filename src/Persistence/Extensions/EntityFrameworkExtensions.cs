using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SmartCoinOS.Persistence.Extensions;

internal static class EntityFrameworkExtensions
{
    public static PropertyBuilder<T> HasEnumStringConversion<T>(this PropertyBuilder<T> propertyBuilder)
    {
        var type = typeof(T);

        if (type.IsEnum)
            return propertyBuilder.HasConversion(
                v => v!.ToString(),
                v => (T)Enum.Parse(type, v ?? string.Empty, true));

        var underlyingType = Nullable.GetUnderlyingType(type);

        return propertyBuilder.HasConversion(
            v => v == null ? null : v.ToString(),
            v => (T)(v == null ? null : Enum.Parse(underlyingType!, v, true))!);
    }

    public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> propertyBuilder)
    {
        var defaultOpts = new JsonSerializerOptions();

        var converter = new ValueConverter<T, string>(
            convertToProviderExpression: obj => JsonSerializer.Serialize(obj, defaultOpts),
            convertFromProviderExpression: str => JsonSerializer.Deserialize<T>(str, defaultOpts)!
        );

        var comparer = new ValueComparer<T>(
            equalsExpression: (left, right) =>
                JsonSerializer.Serialize(left, defaultOpts) == JsonSerializer.Serialize(right, defaultOpts),
            hashCodeExpression: v => v == null ? 0 : JsonSerializer.Serialize(v, defaultOpts).GetHashCode(),
            snapshotExpression: v =>
                JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(v, defaultOpts), defaultOpts)!
        );

        propertyBuilder.HasColumnType("jsonb");
        propertyBuilder.Metadata.SetValueConverter(converter);
        propertyBuilder.Metadata.SetValueComparer(comparer);

        return propertyBuilder;
    }
}
