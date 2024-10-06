using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.BankAccounts;

public sealed record BankAccountDetailsQuery : IQuery<BankAccountDetailsResponse>
{
    [FromRoute(Name = "clientId")]
    public ClientId ClientId { get; init; }

    [FromRoute(Name = "bankAccountId")]
    public BankAccountId BankAccountId { get; init; }
}