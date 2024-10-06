using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.FdtAccounts;

public sealed record CreateFdtAccountCommand : ICommand<EntityId>
{
    [FromRoute(Name = "clientId")]
    public required ClientId ClientId { get; init; }

    public required UserInfoDto UserInfo { get; init; }

    [FromBody]
    public required CreateFdtAccount Account { get; init; }
}

public sealed record CreateFdtAccount
{
    public required string ClientName { get; init; }
    public required string AccountNumber { get; init; }
    public string? Alias { get; init; }
}
