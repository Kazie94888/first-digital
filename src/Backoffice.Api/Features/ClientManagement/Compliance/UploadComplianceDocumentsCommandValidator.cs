using FluentValidation;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Compliance;

public sealed class UploadComplianceDocumentsCommandValidator : AbstractValidator<UploadComplianceDocumentsCommand>
{
    public UploadComplianceDocumentsCommandValidator()
    {
        RuleFor(x => x.Documents).NotEmpty();

        RuleForEach(x => x.Documents).ChildRules(c =>
        {
            c.RuleFor(x => x.FileId).NotEmpty();
            c.RuleFor(x => x.DocumentType).NotEmpty();
        });
    }
}