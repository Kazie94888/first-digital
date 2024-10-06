namespace SmartCoinOS.Domain.Exceptions;

public sealed class EntityInvalidStateException : DomainException
{
    public EntityInvalidStateException(string? message = null) : base(message) { }
}