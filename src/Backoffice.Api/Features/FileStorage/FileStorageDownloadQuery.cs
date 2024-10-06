using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Shared;

namespace SmartCoinOS.Backoffice.Api.Features.FileStorage;

public sealed record FileStorageDownloadQuery : IQuery<FileDto>
{
    [FromRoute(Name = "documentId")]
    public DocumentId DocumentId { get; init; }
}
