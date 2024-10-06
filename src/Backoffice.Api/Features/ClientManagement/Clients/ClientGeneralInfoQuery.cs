using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Clients;

public sealed record ClientGeneralInfoQuery : IQuery<ClientGeneralInfoResponse>
{
    [FromRoute(Name = "clientId")]
    public ClientId ClientId { get; init; }
}
