using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Shared;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.BankAccounts;

public sealed record BankAccountDetailsResponse
{
    public required BankAccountId Id { get; init; }
    public string? Alias { get; init; }
    public required string BankName { get; init; }
    public required EntityVerificationStatus Status { get; init; }
    public required string Beneficiary { get; init; }
    public required string SwiftCode { get; init; }
    public required string Iban { get; init; }
    public required Address BankAddress { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required UserInfoDto CreatedBy { get; init; }
    public required bool Archived { get; init; }
    public DateTimeOffset? ArchivedAt { get; init; }
    public int? OwnBankId { get; init; }
    public int? ThirdPartyBankId { get; init; }
}
