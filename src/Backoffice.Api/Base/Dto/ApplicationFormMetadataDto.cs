namespace SmartCoinOS.Backoffice.Api.Base.Dto;

public sealed record ApplicationFormMetadataDto
{
    public required ApplicationId Id { get; init; }
    public required string PrettyId { get; init; }
}