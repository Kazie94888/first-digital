namespace SmartCoinOS.Domain.Exceptions;

public sealed class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string? message = null) : base(message)
    {

    }
}
