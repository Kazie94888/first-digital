using System.Collections;

namespace SmartCoinOS.Domain.Exceptions;

public sealed class EntityEmptyException : DomainException
{
    private EntityEmptyException() { }

    internal static void ThrowIfEmpty(ICollection items)
    {
        if (items.Count == 0)
            Throw();
    }

    private static void Throw() => throw new EntityEmptyException();
}