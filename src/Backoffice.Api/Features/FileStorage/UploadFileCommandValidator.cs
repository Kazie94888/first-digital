using FluentValidation;

namespace SmartCoinOS.Backoffice.Api.Features.FileStorage;

public sealed class UploadFileCommandValidator : AbstractValidator<UploadFileCommand>
{
    public UploadFileCommandValidator()
    {
        RuleFor(x => x.FormFile).NotNull();
    }
}