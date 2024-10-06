using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.FdtAccounts;

public sealed record FdtAccountListQuery : IQuery<InfoList<FdtAccountListResponse>>
{
    [FromRoute(Name = "clientId")]
    public ClientId ClientId { get; init; }
}
