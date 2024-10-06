using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.FdtAccounts;

public sealed record FdtAccountDetailsQuery : IQuery<FdtAccountDetailsResponse>
{
    [FromRoute(Name = "clientId")]
    public required ClientId ClientId { get; init; }

    [FromRoute(Name = "fdtAccountId")]
    public required FdtAccountId FdtAccountId { get; init; }
}
