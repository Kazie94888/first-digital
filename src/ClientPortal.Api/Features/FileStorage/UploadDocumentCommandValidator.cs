using FluentValidation;
using SmartCoinOS.Domain.Shared;

namespace SmartCoinOS.ClientPortal.Api.Features.FileStorage;

internal sealed class UploadDocumentCommandValidator : AbstractValidator<UploadDocumentCommand>
{
    public UploadDocumentCommandValidator()
    {
        RuleFor(x => x.FormFile).NotNull();
        
        RuleFor(x => x.FormFile).Must(x => Document.Validate(x.ContentType, x.Length).IsSuccess)
            .WithMessage("Invalid file type or size");
    }
}