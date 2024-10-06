using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Shared;

namespace SmartCoinOS.ClientPortal.Api.Features.CompanyInformation.Documents;

internal class CreateCompanyDocumentCommand : ICommand<EntityId>
{
    [FromRoute(Name = "documentId")]
    public required DocumentId DocumentId { get; init; }
    
    [FromBody]
    public required CompanyDocumentRequest Document { get; init; }
    
    public required UserInfoDto UserInfo { get; init; }
}

internal sealed record CompanyDocumentRequest
{
    public string? FileName { get; init; }
    public required string DocumentType { get; init; }
}