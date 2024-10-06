using AWSServiceProvider.Services.S3;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Shared;
using SmartCoinOS.Extensions;
using SmartCoinOS.Persistence;

namespace SmartCoinOS.ClientPortal.Api.Features.FileStorage;

internal sealed class UploadDocumentCommandHandler : ICommandHandler<UploadDocumentCommand, UploadDocumentResponse>
{
    private readonly DataContext _context;
    private readonly IFileService _fileService;

    public UploadDocumentCommandHandler(DataContext context, IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<Result<UploadDocumentResponse>> Handle(UploadDocumentCommand request,
        CancellationToken cancellationToken)
    {
        var documentId = DocumentId.New();
        var extension = Path.GetExtension(request.FormFile.FileName);
        var uploadFileDto = new UploadFileDto
        {
            Key = $"{documentId}{extension}",
            FileName = request.FormFile.FileName,
            ContentType = request.FormFile.ContentType,
            Length = request.FormFile.Length,
            ByteArray = await request.FormFile.ToByteArrayAsync(),
        };

        var uploadedSuccessfully = await _fileService.UploadAsync(uploadFileDto, cancellationToken);
        if (!uploadedSuccessfully)
            return Result.Fail("Failed to upload the document");

        var tempDoc = Document.CreateTempDocument(documentId,
            uploadFileDto.Key,
            uploadFileDto.FileName,
            uploadFileDto.ContentType,
            uploadFileDto.Length,
            request.UserInfo.ClientId,
            request.UserInfo);
        _context.Documents.Add(tempDoc);

        var client = await _context.Clients.FirstAsync(x => x.Id == request.UserInfo.ClientId, cancellationToken);

        client.AddDocument(tempDoc.Id, request.FormFile.FileName, request.FormFile.ContentType, request.UserInfo);

        _context.Update(client);

        return Result.Ok(new UploadDocumentResponse
        {
            Id = tempDoc.Id,
            Name = uploadFileDto.FileName
        });
    }
}
