using System.Globalization;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace SmartCoinOS.Extensions.Fiql.Conditions;

internal sealed record ComparerCondition(string Property, string Operator, string Value) : ICondition
{
    internal static class Operators
    {
        internal const string Equal = "==";
        internal const string NotEqual = "!=";
        internal const string LessThan = "=lt=";
        internal const string LessThanOrEqual = "=le=";
        internal const string GreaterThan = "=gt=";
        internal const string GreaterThanOrEqual = "=ge=";
        internal const string IsNull = "=is-null=";
        internal const string In = "=in=";
        internal const string Out = "=out=";
        internal const string Contains = "=contains=";

        public static readonly string[] All =
        [
            Equal,
            NotEqual,
            LessThan,
            LessThanOrEqual,
            GreaterThan,
            GreaterThanOrEqual,
            IsNull,
            In,
            Out,
            Contains
        ];
    }

    public static readonly Regex OperatorPatternRegex =
        new($"{string.Join('|', Operators.All)}|=(.*?)=", RegexOptions.Compiled);

    public override string ToString()
    {
        return $"{Property}{Operator}{Value}";
    }

    public Expression BuildExpression(ParameterExpression parameter)
    {
        var property = GetProperty(parameter, Property);
        object[] values;
        Expression expression;

        switch (Operator)
        {
            case Operators.Equal:
                expression = Expression.Equal(property, Expression.Constant(ConvertType(Value, property.Type)));
                break;
            case Operators.NotEqual:
                expression = Expression.NotEqual(property, Expression.Constant(ConvertType(Value, property.Type)));
                break;
            case Operators.LessThan:
                expression = Expression.LessThan(property, Expression.Constant(ConvertType(Value, property.Type)));
                break;
            case Operators.LessThanOrEqual:
                expression =
                    Expression.LessThanOrEqual(property, Expression.Constant(ConvertType(Value, property.Type)));
                break;
            case Operators.GreaterThan:
                expression = Expression.GreaterThan(property, Expression.Constant(ConvertType(Value, property.Type)));
                break;
            case Operators.GreaterThanOrEqual:
                expression =
                    Expression.GreaterThanOrEqual(property, Expression.Constant(ConvertType(Value, property.Type)));
                break;
            case Operators.IsNull:
                expression = Expression.Equal(property, Expression.Constant(null));
                break;
            case Operators.In:
                values = Value.Trim('(', ')').Split(',').Select(val => ConvertType(val, property.Type)).ToArray();
                expression = values.Aggregate((Expression?)null, (current, value) =>
                    current == null
                        ? Expression.Equal(property, Expression.Constant(value))
                        : Expression.OrElse(current, Expression.Equal(property, Expression.Constant(value))))!;
                break;
            case Operators.Out:
                values = Value.Trim('(', ')').Split(',').Select(val => ConvertType(val, property.Type)).ToArray();
                expression = values.Aggregate((Expression?)null, (current, value) =>
                    current == null
                        ? Expression.NotEqual(property, Expression.Constant(value))
                        : Expression.AndAlso(current, Expression.NotEqual(property, Expression.Constant(value))))!;
                break;
            case Operators.Contains:
                if (property.Type != typeof(string))
                    throw new InvalidOperationException("Contains can only be used with string properties.");
                expression = Expression.Call(property, "Contains", null, Expression.Constant(Value));
                break;
            default:
                throw new NotSupportedException($"Operator '{Operator}' is not supported.");
        }

        return expression;
    }

    private static MemberExpression GetProperty(ParameterExpression parameter, string propertyName)
    {
        var properties = propertyName.Split('.');
        return properties.Aggregate<string, MemberExpression?>(null,
            (current, subPropertyName) => Expression.Property(current ?? (Expression)parameter, subPropertyName))!;
    }

    private static object ConvertType(string value, Type type)
    {
        if (type == typeof(DateTime) || type == typeof(DateTime?))
            return DateTime.TryParse(value, DateTimeFormatInfo.CurrentInfo, DateTimeStyles.AdjustToUniversal,
                out var result)
                ? DateTime.SpecifyKind(result, DateTimeKind.Utc)
                : throw new InvalidOperationException($"Cannot parse {value} into {type}");

        if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?))
            return DateTimeOffset.TryParse(value, DateTimeFormatInfo.CurrentInfo, DateTimeStyles.AdjustToUniversal,
                out var result)
                ? result
                : throw new InvalidOperationException($"Cannot parse {value} into {type}");

        var normalizedFromKebabCase = value.Replace("-", string.Empty);

        if (type.IsEnum)
            return Enum.Parse(type, normalizedFromKebabCase, ignoreCase: true);

        var underlyingType = Nullable.GetUnderlyingType(type);
        if (underlyingType is { IsEnum: true })
            return Enum.Parse(underlyingType, normalizedFromKebabCase, ignoreCase: true);

        return Convert.ChangeType(value, type);
    }
}