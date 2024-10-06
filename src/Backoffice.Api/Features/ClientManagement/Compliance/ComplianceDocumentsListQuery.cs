using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Shared;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Compliance;

public sealed record ComplianceDocumentsListQuery : IQuery<InfoList<ComplianceDocumentsItem>>
{
    [FromRoute(Name = "clientId")]
    public required ClientId ClientId { get; init; }
}

public sealed record ComplianceDocumentsItem
{
    public required DocumentId Id { get; init; }
    public required string OriginalFileName { get; init; }
    public required string DocumentType { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
}