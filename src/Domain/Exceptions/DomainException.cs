namespace SmartCoinOS.Domain.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(string? message = null) : base(message) { }
}
