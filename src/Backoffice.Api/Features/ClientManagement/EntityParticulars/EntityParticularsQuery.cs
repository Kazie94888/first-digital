using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.EntityParticulars;

public sealed record EntityParticularsQuery : IQuery<EntityParticularsResponse>
{
    [FromRoute(Name = "clientId")]
    public ClientId ClientId { get; init; }
}
