using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.BankAccounts;

public sealed record SetBankAccountsFormCommand : ICommand<ApplicationFormMetadataDto>
{
    [FromRoute(Name = "applicationId")]
    public required ApplicationId ApplicationId { get; init; }

    [FromBody]
    public required List<SetBankAccountsRequest> BankAccounts { get; init; } = [];

    public required UserInfoDto UserInfo { get; init; }
}

public sealed record SetBankAccountsRequest
{
    public required string Beneficiary { get; init; }
    public required string BankName { get; init; }
    public required string Iban { get; init; }
    public required string Swift { get; init; }
    public int? SmartTrustOwnBankId { get; init; }
    public int? SmartTrustThirdPartyBankId { get; init; }
    public required string Street { get; init; }
    public required string City { get; init; }
    public required string PostalCode { get; init; }
    public string? State { get; init; }
    public required string Country { get; init; }
    public DateTimeOffset? CreatedAt { get; init; }

    public required List<DocumentDto> Documents { get; init; } = [];
}
