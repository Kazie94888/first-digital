namespace SmartCoinOS.Domain.Exceptions;

public sealed class CreateEntityException : DomainException
{
    public CreateEntityException(string? message = null) : base(message) { }
}