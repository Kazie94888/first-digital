namespace SmartCoinOS.Backoffice.Api.Base.Dto;

public sealed record SignatureDto
{
    public required string Address { get; init; }
    public string? Alias { get; init; }
    public required DateTimeOffset SubmissionDate { get; init; }
}