using AWSServiceProvider.Services.S3;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Persistence;

namespace SmartCoinOS.ClientPortal.Api.Features.FileStorage;

internal sealed class DownloadDocumentQueryHandler : IQueryHandler<DownloadDocumentQuery, FileDto>
{
    private readonly ReadOnlyDataContext _context;
    private readonly IFileService _fileService;

    public DownloadDocumentQueryHandler(ReadOnlyDataContext context, IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<Result<FileDto>> Handle(DownloadDocumentQuery request, CancellationToken cancellationToken)
    {
        var document = await _context.Documents.FirstOrDefaultAsync(x =>
                x.Id == request.DocumentId && x.ClientId == request.UserInfo.ClientId,
            cancellationToken);
        if (document == null)
            return Result.Fail("Document was not found.");

        var fileBytes = await _fileService.DownloadAsync(document.Key, cancellationToken);

        return Result.Ok(new FileDto
        {
            Bytes = fileBytes,
            MimeType = document.ContentType,
            Name = document.FileName
        });
    }
}