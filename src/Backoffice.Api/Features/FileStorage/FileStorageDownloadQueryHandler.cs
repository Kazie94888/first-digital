using AWSServiceProvider.Services.S3;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.FileStorage;

internal sealed class FileStorageDownloadQueryHandler : IQueryHandler<FileStorageDownloadQuery, FileDto>
{
    private readonly DataContext _context;
    private readonly IFileService _fileService;
    public FileStorageDownloadQueryHandler(DataContext context, IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<Result<FileDto>> Handle(FileStorageDownloadQuery request, CancellationToken cancellationToken)
    {
        var document = await _context.Documents.FirstOrDefaultAsync(x => x.Id == request.DocumentId, cancellationToken);
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