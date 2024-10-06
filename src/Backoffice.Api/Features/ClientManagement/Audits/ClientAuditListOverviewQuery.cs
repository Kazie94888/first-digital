using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Audits;

public sealed record ClientAuditListOverviewQuery : IQuery<InfoList<ClientAuditListOverviewResponse>>
{
    [FromRoute(Name = "clientId")]
    public required ClientId ClientId { get; init; }
}
