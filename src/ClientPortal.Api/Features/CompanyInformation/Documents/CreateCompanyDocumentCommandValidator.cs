using FluentValidation;

namespace SmartCoinOS.ClientPortal.Api.Features.CompanyInformation.Documents;

internal sealed class CreateCompanyDocumentCommandValidator : AbstractValidator<CreateCompanyDocumentCommand>
{
    public CreateCompanyDocumentCommandValidator()
    {
        RuleFor(x => x.Document.DocumentType).NotEmpty();
    }
}