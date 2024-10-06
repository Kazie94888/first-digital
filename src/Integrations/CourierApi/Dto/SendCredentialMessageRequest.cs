namespace SmartCoinOS.Integrations.CourierApi.Dto;

public sealed record SendCredentialMessageRequest
{
    public required CredentialMessageRequestTo To { get; init; }
    public required string Template { get; init; }
    public required CredentialMessageRequestData Data { get; init; }
}

public sealed record CredentialMessageRequestData
{
    public required string ClientEmail { get; init; }
    public required string TemporaryPassword { get; init; }
    public required string FullName { get; init; }
}

public sealed record CredentialMessageRequestTo
{
    public required string Email { get; init; }
}

public sealed record SendCredentialsEmailRequest
{
    public required SendCredentialMessageRequest Message { get; init; }
}