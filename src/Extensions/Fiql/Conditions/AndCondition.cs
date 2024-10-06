﻿using System.Linq.Expressions;

namespace SmartCoinOS.Extensions.Fiql.Conditions;
internal sealed record AndCondition(ICondition Left, ICondition Right) : ICondition
{
    internal const char Operator = ';';

    public override string ToString()
    {
        return $"{FiqlParser.GroupedConditionOpen}{Left}{Operator}{Right}{FiqlParser.GroupedConditionClose})";
    }

    public Expression BuildExpression(ParameterExpression parameter)
    {
        return Expression.AndAlso(Left.BuildExpression(parameter), Right.BuildExpression(parameter));
    }
}