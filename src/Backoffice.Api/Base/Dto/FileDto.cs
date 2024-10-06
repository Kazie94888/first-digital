namespace SmartCoinOS.Backoffice.Api.Base.Dto;

public sealed record FileDto
{
    public required byte[] Bytes { get; init; }
    public required string MimeType { get; init; }
    public required string Name { get; init; }
}
