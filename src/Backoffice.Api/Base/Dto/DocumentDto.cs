using SmartCoinOS.Domain.Application;
using SmartCoinOS.Domain.Shared;

namespace SmartCoinOS.Backoffice.Api.Base.Dto;

public sealed record DocumentDto
{
    public required DocumentId FileId { get; init; }
    public required string DocumentType { get; init; }
    public string? FileName { get; init; }

    public static implicit operator DocumentDto(ApplicationDocument applicationDoc)
    {
        return new DocumentDto
        {
            FileId = applicationDoc.DocumentId,
            DocumentType = applicationDoc.DocumentType,
            FileName = applicationDoc.FileName,
        };
    }

    public static implicit operator ApplicationDocument(DocumentDto dto)
    {
        return new ApplicationDocument
        {
            DocumentId = dto.FileId,
            DocumentType = dto.DocumentType,
            FileName = dto.FileName
        };
    }
}