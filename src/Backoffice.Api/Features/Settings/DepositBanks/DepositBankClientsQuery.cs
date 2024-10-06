using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Application;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Deposit;

namespace SmartCoinOS.Backoffice.Api.Features.Settings.DepositBanks;

public sealed record DepositBankClientsQuery : PagedListFiqlQuery, IQuery<InfoPagedList<ClientsDepositBank>>
{
    [FromRoute(Name = "id")]
    public required DepositBankId Id { get; init; }
}

public sealed record ClientsDepositBank
{
    public required ClientId Id { get; init; }
    public required string LegalEntityName { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required int Orders { get; init; }
}
