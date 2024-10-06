using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Compliance;

public sealed record UploadComplianceDocumentsCommand : ICommand<List<DocumentDto>>
{
    [FromRoute(Name = "clientId")]
    public required ClientId ClientId { get; init; }

    [FromBody]
    public required List<DocumentDto> Documents { get; init; } = [];

    public required UserInfoDto UserInfo { get; init; }
}
