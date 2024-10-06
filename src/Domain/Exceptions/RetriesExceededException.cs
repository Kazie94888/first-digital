namespace SmartCoinOS.Domain.Exceptions;

public sealed class RetriesExceededException : DomainException
{
    public RetriesExceededException(string? message = null) : base(message) { }
}