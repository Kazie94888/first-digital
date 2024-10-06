namespace SmartCoinOS.Domain.Clients;

public sealed record CompanyContact
{
    public required string Email { get; init; }
    public required string Phone { get; init; }
}
