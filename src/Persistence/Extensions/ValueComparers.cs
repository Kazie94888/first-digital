using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace SmartCoinOS.Persistence.Extensions;

public static class ValueComparers<T> where T : class
{
    public static ValueComparer<IReadOnlyList<T>?> GetCollectionComparerByValue()
    {
        return new ValueComparer<IReadOnlyList<T>?>(
            equalsExpression: (c1, c2) => c1!.SequenceEqual(c2!),
            hashCodeExpression: c => c!.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            snapshotExpression: c => c!.ToList()
        );
    }
}