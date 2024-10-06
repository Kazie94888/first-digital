using AWSServiceProvider.Services.S3;
using FluentResults;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Shared;
using SmartCoinOS.Extensions;

namespace SmartCoinOS.Backoffice.Api.Features.FileStorage;

internal sealed class UploadFileCommandHandler : ICommandHandler<UploadFileCommand, UploadFileResponse>
{
    private readonly DataContext _context;
    private readonly IFileService _fileService;

    public UploadFileCommandHandler(DataContext context, IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<Result<UploadFileResponse>> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        var documentId = DocumentId.New();
        var extention = Path.GetExtension(request.FormFile.FileName);

        var uploadFileDto = new UploadFileDto
        {
            Key = $"{documentId}{extention}",
            FileName = request.FormFile.FileName,
            ContentType = request.FormFile.ContentType,
            Length = request.FormFile.Length,
            ByteArray = await request.FormFile.ToByteArrayAsync(),
        };

        var uploadedSuccessfuly = await _fileService.UploadAsync(uploadFileDto, cancellationToken);
        if (!uploadedSuccessfuly)
            return Result.Fail("Failed to upload the document");

        var appDocument = Document.CreateTempDocument(documentId,
                uploadFileDto.Key,
                uploadFileDto.FileName,
                uploadFileDto.ContentType,
                uploadFileDto.Length,
                null,
                request.UserInfo);

        _context.Documents.Add(appDocument);

        var fileResult = new UploadFileResponse
        {
            Id = appDocument.Id,
            Name = uploadFileDto.FileName
        };

        return fileResult;
    }
}