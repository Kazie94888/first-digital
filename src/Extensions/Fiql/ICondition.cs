using System.Linq.Expressions;

namespace SmartCoinOS.Extensions.Fiql;

public interface ICondition
{
    Expression BuildExpression(ParameterExpression parameter);
}

