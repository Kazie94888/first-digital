using System.Linq.Expressions;

namespace SmartCoinOS.Extensions.Fiql;

public static class QueryableExtensions
{
    public static IQueryable<T> Filter<T>(this IQueryable<T> query, IPagedListFiqlQuery request)
    {
        return string.IsNullOrWhiteSpace(request.FiqlQuery)
            ? query
            : query.Where(FiqlParser.ParseToExpression<T>(request.FiqlQuery));
    }

    public static IQueryable<T> SortAndPage<T>(this IQueryable<T> buildQuery, IPagedListQuery request)
    {
        if (!string.IsNullOrWhiteSpace(request.SortQuery))
            buildQuery = buildQuery.Sort(request.SortQuery);

        if (request.Skip != 0)
            buildQuery = buildQuery.Skip(request.Skip);

        if (request.Take != int.MaxValue)
            buildQuery = buildQuery.Take(request.Take);

        return buildQuery;
    }

    public static IQueryable<T> Sort<T>(this IQueryable<T> buildQuery, string sortQuery)
    {
        var sortParameters = ParseSortQuery(sortQuery);

        for (var i = 0; i < sortParameters.Count; i++)
        {
            var (propertyName, direction) = sortParameters[i];
            buildQuery = buildQuery.Provider.CreateQuery<T>(CreateQuery(buildQuery, propertyName, direction, isSubQuery: i != 0));
        }

        return buildQuery;
    }

    private static MethodCallExpression CreateQuery<T>(
        IQueryable<T> source, string propertyName,
        SortDirection sortDirection, bool isSubQuery = false)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
            throw new ArgumentException("Property name is incorrect", nameof(propertyName));

        var type = typeof(T);
        var parameter = Expression.Parameter(type, "x");
        var property = GetProperty(parameter, propertyName);
        var lambda = Expression.Lambda(property, parameter);

        var methodName = sortDirection switch
        {
            SortDirection.Ascending when !isSubQuery => "OrderBy",
            SortDirection.Ascending when isSubQuery => "ThenBy",
            SortDirection.Descending when !isSubQuery => "OrderByDescending",
            SortDirection.Descending when isSubQuery => "ThenByDescending",
            _ => throw new ArgumentException("Invalid sorting parameters")
        };

        var types = new[] { type, property.Type };
        var methodCall = Expression.Call(typeof(Queryable), methodName, types, source.Expression, lambda);
        return methodCall;
    }

    private static List<SortEntry> ParseSortQuery(string sortQuery)
    {
        var entries = new List<SortEntry>();
        var sortFields = sortQuery.Split(',');

        foreach (var sortField in sortFields)
        {
            var parts = sortField.Split(' ');

            if (parts.Length != 2)
                throw new ArgumentException("Sort query is incorrect", nameof(sortQuery));

            var direction = parts[1] switch
            {
                "asc" => SortDirection.Ascending,
                "desc" => SortDirection.Descending,
                _ => throw new ArgumentException("Incorrect sort direction", nameof(sortQuery))
            };

            entries.Add(new SortEntry(parts[0], direction));
        }

        return entries;
    }

    private static MemberExpression GetProperty(ParameterExpression parameter, string propertyName)
    {
        var properties = propertyName.Split('.');
        return properties.Aggregate<string, MemberExpression?>(null,
            (current, subPropertyName) => Expression.Property(current ?? (Expression)parameter, subPropertyName))!;
    }

    private sealed record SortEntry(string PropertyName, SortDirection Direction);

    private enum SortDirection
    {
        Ascending,
        Descending
    }
}