using System.Linq.Expressions;
using System.Text.Json;

namespace SmartCoinOS.Domain.Base;

public static class InfoListExtension
{
    public static InfoPagedList<T> Sorted<T>(this InfoPagedList<T> theList, Expression<Func<T, object>> orderBy, bool asc = true) where T : class
    {
        var sortedPropName = "";

        if (orderBy.Body is MemberExpression expression)
            sortedPropName = expression.Member.Name;
        else if (orderBy.Body is UnaryExpression unaryExpression)
            sortedPropName = ((MemberExpression)unaryExpression.Operand).Member.Name;

        var nameToCamelCase = JsonNamingPolicy.CamelCase.ConvertName(sortedPropName);

        var direction = asc ? "asc" : "desc";
        theList.AddSort($"{nameToCamelCase} {direction}");

        return theList;
    }

    public static InfoList<T> Sorted<T>(this InfoList<T> theList, Expression<Func<T, object>> orderBy, bool asc = true) where T : class
    {
        var sortedPropName = "";

        if (orderBy.Body is MemberExpression expression)
            sortedPropName = expression.Member.Name;
        else if (orderBy.Body is UnaryExpression unaryExpression)
            sortedPropName = ((MemberExpression)unaryExpression.Operand).Member.Name;

        var nameToCamelCase = JsonNamingPolicy.CamelCase.ConvertName(sortedPropName);

        var direction = asc ? "asc" : "desc";
        theList.AddSort($"{nameToCamelCase} {direction}");

        return theList;
    }
}
