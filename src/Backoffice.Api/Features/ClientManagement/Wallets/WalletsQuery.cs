using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Wallets;

public sealed record WalletsQuery : IQuery<InfoList<WalletsResponse>>
{
    [FromRoute(Name = "clientId")]
    public ClientId ClientId { get; init; }
}