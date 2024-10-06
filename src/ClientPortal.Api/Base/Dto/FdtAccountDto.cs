namespace SmartCoinOS.ClientPortal.Api.Base.Dto;

internal sealed record FdtAccountDto
{
    public required string Name { get; init; }
    public required string Number { get; init; }
}