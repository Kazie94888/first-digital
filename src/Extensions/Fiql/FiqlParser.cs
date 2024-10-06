using System.Linq.Expressions;
using SmartCoinOS.Extensions.Fiql.Conditions;

namespace SmartCoinOS.Extensions.Fiql;

public static class FiqlParser
{
    private const int _maxDepth = 10;
    public const char GroupedConditionOpen = '(';
    public const char GroupedConditionClose = ')';

    public static Expression<Func<T, bool>> ParseToExpression<T>(
        string filter)
    {
        var condition = Parse(filter);

        var parameter = Expression.Parameter(typeof(T), "x");
        var expression = condition.BuildExpression(parameter);
        return Expression.Lambda<Func<T, bool>>(expression, parameter);
    }

    private static ICondition Parse(string filter) => ParseInternal(filter);

    private static void QuerySyntaxGuard(string filter, int depth = 0)
    {
        FiqlParserException.ThrowIf(depth >= _maxDepth, "Unable to parse filter, string is too big");
        FiqlParserException.ThrowIf(filter.Length == 0, "Unable to find operator.");
    }

    private static ICondition ParseInternal(string filter, int i = 0, int depth = 0)
    {
        QuerySyntaxGuard(filter, depth);

        ICondition condition;
        if (filter[i] == GroupedConditionOpen)
        {
            var closeIndex = NextGroupedConditionCloseIndex(filter, i);

            condition = ParseInternal(filter[(i + 1)..closeIndex], depth: depth + 1);
            i = closeIndex + 1;
        }
        else
        {
            var operatorMatch = ComparerCondition.OperatorPatternRegex.Match(filter, i);

            FiqlParserException.ThrowIf(!operatorMatch.Success || operatorMatch.Index <= 0, "Unable to find operator.");

            var property = filter[i..operatorMatch.Index];
            var @operator = operatorMatch.Value;
            i += property.Length + @operator.Length;

            // Value ends when:
            // 1 - ';', ',' symbols are met;
            // 2 - some operators use ',' as a list separator. in that case value ends with ')' symbol;
            // 3 - end of the string.

            string value;
            if (@operator is ComparerCondition.Operators.In or ComparerCondition.Operators.Out)
            {
                FiqlParserException.ThrowIf(filter[i] != '(',
                    $"{ComparerCondition.Operators.In} and {ComparerCondition.Operators.Out} should be compared to a list of items in ( ).");

                var valueEnd = filter.IndexOf(')', i);

                FiqlParserException.ThrowIf(valueEnd < 0,
                    $"{ComparerCondition.Operators.In} and {ComparerCondition.Operators.Out} should be compared to a list of items in ( ).");

                value = filter[i..(valueEnd + 1)];
            }
            else
            {
                char[] valueEndIndicators =
                [
                    AndCondition.Operator,
                    OrCondition.Operator
                ];
                var valueEnd = filter.IndexOfAny(valueEndIndicators, i);
                if (valueEnd < 0)
                    value = filter[i..];
                else
                    value = filter[i..valueEnd];
            }

            string[] operatorsWithoutValue =
            [
                ComparerCondition.Operators.IsNull
            ];

            FiqlParserException.ThrowIf(string.IsNullOrWhiteSpace(value) && !operatorsWithoutValue.Contains(@operator),
                $"Value is empty for property {property}");

            condition = new ComparerCondition(property, @operator, value);

            i += value.Length;
        }

        // Property ends when operator starts. Operators starts with '=' symbol
        if (i >= filter.Length)
            return condition;

        var nextCondition = ParseInternal(filter, i + 1, depth + 1);

        return filter[i] switch
        {
            AndCondition.Operator => new AndCondition(condition, nextCondition),
            OrCondition.Operator => new OrCondition(condition, nextCondition),
            _ => throw new FiqlParserException("Unable to parse query")
        };
    }

    private static int NextGroupedConditionCloseIndex(string filter, int index)
    {
        var openParenthesisCount = 1;
        var i = index + 1;

        while (openParenthesisCount > 0 && i < filter.Length)
        {
            if (filter[i] == GroupedConditionOpen)
                openParenthesisCount++;
            else if (filter[i] == GroupedConditionClose)
                openParenthesisCount--;
            i++;
        }

        if (openParenthesisCount > 0)
            throw new FiqlParserException($"Unable to find close operator '{GroupedConditionClose}'.");

        return i - 1;
    }
}
