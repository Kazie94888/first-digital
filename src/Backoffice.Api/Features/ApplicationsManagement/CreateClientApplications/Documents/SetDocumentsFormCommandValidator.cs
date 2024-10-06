using FluentValidation;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Services;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.Documents;

public sealed class SetDocumentsFormCommandValidator : AbstractValidator<SetDocumentsFormCommand>
{
    public SetDocumentsFormCommandValidator()
    {
        RuleFor(x => x.Documents).NotEmpty();

        var dueDiligenceValidator = new DueDiligenceFormDocumentValidator();
        RuleFor(x => x.Documents.MemorandumOfAssociation).SetValidator(dueDiligenceValidator);
        RuleFor(x => x.Documents.BankStatement).SetValidator(dueDiligenceValidator);
        RuleFor(x => x.Documents.CertificateOfIncumbency).SetValidator(dueDiligenceValidator);
        RuleFor(x => x.Documents.GroupOwnershipAndStructure).SetValidator(dueDiligenceValidator);
        RuleFor(x => x.Documents.CertificateOfIncAndNameChange).SetValidator(dueDiligenceValidator);
        RuleFor(x => x.Documents.RegisterOfShareholder).SetValidator(dueDiligenceValidator);
        RuleFor(x => x.Documents.RegisterOfDirectors).SetValidator(dueDiligenceValidator);
        RuleFor(x => x.Documents.BoardResolutionAppointingAdditionalUsers).SetValidator(dueDiligenceValidator);

        RuleForEach(x => x.Documents.AdditionalDocuments).Must(x => DocumentTypeService.Instance.IsValid(x.DocumentType));
    }

    private sealed class DueDiligenceFormDocumentValidator : AbstractValidator<DocumentDto>
    {
        public DueDiligenceFormDocumentValidator()
        {
            RuleFor(x => x).NotEmpty().Must(x => DocumentTypeService.Instance.IsDueDiligence(x.DocumentType));
        }
    }
}