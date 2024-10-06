using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Deposit;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Overview;

public sealed record OverviewDepositListQuery : IQuery<InfoList<OverviewDepositListItem>>
{
    [FromRoute(Name = "clientId")]
    public required ClientId Id { get; init; }
}

public sealed record OverviewDepositListItem
{
    public required DepositBankId Id { get; init; }
    public required string Name { get; init; }
    public required string Iban { get; init; }
    public required bool IsDefault { get; init; }
}
