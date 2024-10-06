namespace SmartCoinOS.Domain.Exceptions;

public sealed class EntityNullException : DomainException
{
    private EntityNullException() { }
    internal EntityNullException(string message) : base(message) { }

    internal static void ThrowIfNull(object? argument, string paramName)
    {
        if (argument is null)
            Throw(paramName);
    }

    private static void Throw(string paramName) => throw new EntityNullException(paramName);
}
