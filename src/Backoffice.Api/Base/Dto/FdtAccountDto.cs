namespace SmartCoinOS.Backoffice.Api.Base.Dto;

public sealed record FdtAccountDto
{
    public required string Name { get; init; }
    public required string Number { get; init; }
}