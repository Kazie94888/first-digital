using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Application.Enums;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.Documents;

internal sealed class DocumentsFormQueryHandler : IQueryHandler<DocumentsFormQuery, ApplicationFormDto>
{
    private readonly ReadOnlyDataContext _context;

    public DocumentsFormQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<ApplicationFormDto>> Handle(DocumentsFormQuery request, CancellationToken cancellationToken)
    {
        var application = await _context.Applications.FirstAsync(x => x.Id == request.ApplicationId, cancellationToken);

        var documentsForm = new VerifiableSectionDto<SetDocumentsRequest>();
        if (application.Documents is not null)
        {
            var appDocs = application.Documents.OnboardingDocument;
            documentsForm.Set(appDocs.IsVerified(), new SetDocumentsRequest
            {
                MemorandumOfAssociation = appDocs.MemorandumOfAssociation,
                BankStatement = appDocs.BankStatement,
                CertificateOfIncumbency = appDocs.CertificateOfIncumbency,
                GroupOwnershipAndStructure = appDocs.GroupOwnershipAndStructure,
                CertificateOfIncAndNameChange = appDocs.CertificateOfIncAndNameChange,
                RegisterOfShareholder = appDocs.RegisterOfShareholder,
                RegisterOfDirectors = appDocs.RegisterOfDirectors,
                BoardResolutionAppointingAdditionalUsers = appDocs.BoardResolutionAppointingAdditionalUsers,
                AdditionalDocuments = appDocs.AdditionalDocuments.Select(x => (DocumentDto)x).ToList(),

                CreatedAt = appDocs.CreatedAt
            });
        }

        var appForm = new ApplicationFormDto(ApplicationFormType.Documents);
        appForm.AddSection(ApplicationFormNames.DocumentRecords.DueDiligenceRecords, documentsForm);

        return Result.Ok(appForm);
    }
}
