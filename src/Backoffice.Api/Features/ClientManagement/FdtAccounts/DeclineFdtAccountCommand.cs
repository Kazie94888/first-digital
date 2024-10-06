using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.FdtAccounts;

internal sealed record DeclineFdtAccountCommand : ICommand<EntityId>
{
    [FromRoute(Name = "clientId")]
    public ClientId ClientId { get; init; }

    [FromRoute(Name = "fdtAccountId")]
    public FdtAccountId FdtAccountId { get; init; }

    public required UserInfoDto UserInfo { get; init; }
}
