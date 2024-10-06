using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Application;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.BankAccounts;

public sealed record BankAccountOrderListQuery : PagedListQuery, IQuery<InfoPagedList<BankAccountOrderListResponse>>
{
    [FromRoute(Name = "clientId")]
    public required ClientId ClientId { get; init; }

    [FromRoute(Name = "bankAccountId")]
    public required BankAccountId BankAccountId { get; init; }
}