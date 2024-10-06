using SmartCoinOS.Domain.Deposit;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Overview;

public sealed record ClientOverviewResponse
{
    public required string LegalEntityName { get; init; }
    public required string JurisdictionOfInc { get; init; }
    public required string RegistrationNumber { get; init; }
    public required DateOnly DateOfInc { get; init; }
    public required string RegistrationAddress { get; init; }
    public required bool EntityParticularsVerified { get; init; }
    public required bool WalletsVerified { get; init; }
    public required bool BankAccountsVerified { get; init; }
    public DepositBankOverview? DepositBank { get; init; }
}

public sealed record DepositBankOverview
{
    public required DepositBankId Id { get; init; }
    public required string BankName { get; init; }
    public required string Iban { get; init; }
}