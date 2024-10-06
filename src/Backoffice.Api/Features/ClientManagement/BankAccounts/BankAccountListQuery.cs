using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.BankAccounts;

public sealed record BankAccountListQuery : IQuery<InfoList<BankAccountListResponse>>
{
    [FromRoute(Name = "clientId")]
    public ClientId ClientId { get; init; }
}