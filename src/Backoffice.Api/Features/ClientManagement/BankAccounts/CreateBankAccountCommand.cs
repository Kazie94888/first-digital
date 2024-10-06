using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.BankAccounts;

public sealed record CreateBankAccountCommand : ICommand<EntityId>
{
    [FromRoute(Name = "clientId")]
    public ClientId ClientId { get; init; }

    [FromBody]
    public required CreateBankAccount BankAccount { get; init; }

    public required UserInfoDto UserInfo { get; init; }
}

public sealed record CreateBankAccount
{
    public required BankAccountDetails BankAccountDetails { get; init; }
    public required BankAddress BankAddress { get; init; }
}

public sealed record BankAccountDetails
{
    public required string Beneficiary { get; init; }
    public required string Iban { get; init; }
    public required string BankName { get; init; }
    public required string SwiftCode { get; init; }
    public string? Alias { get; init; }
    public int? SmartTrustOwnBankId { get; init; }
    public int? SmartTrustThirdPartyBankId { get; init; }
}

public sealed record BankAddress
{
    public required string Street { get; init; }
    public required string PostalCode { get; init; }
    public required string City { get; init; }
    public string? State { get; init; }
    public required string Country { get; init; }
}