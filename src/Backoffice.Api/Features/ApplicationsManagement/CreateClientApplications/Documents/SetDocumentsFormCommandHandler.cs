using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Application;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.Documents;

internal sealed class SetDocumentsFormCommandHandler
    : ICommandHandler<SetDocumentsFormCommand, ApplicationFormMetadataDto>
{
    private readonly DataContext _context;

    public SetDocumentsFormCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<ApplicationFormMetadataDto>> Handle(SetDocumentsFormCommand request,
        CancellationToken cancellationToken)
    {
        var application = await _context.Applications.FirstAsync(x => x.Id == request.ApplicationId, cancellationToken);

        var rDocs = request.Documents;
        
        var documentRecord = new OnboardingDocumentRecord
        {
            MemorandumOfAssociation = rDocs.MemorandumOfAssociation,
            BankStatement = rDocs.BankStatement,
            CertificateOfIncumbency = rDocs.CertificateOfIncumbency,
            GroupOwnershipAndStructure = rDocs.GroupOwnershipAndStructure,
            CertificateOfIncAndNameChange = rDocs.CertificateOfIncAndNameChange,
            RegisterOfShareholder = rDocs.RegisterOfShareholder,
            RegisterOfDirectors = rDocs.RegisterOfDirectors,
            BoardResolutionAppointingAdditionalUsers = rDocs.BoardResolutionAppointingAdditionalUsers,
            AdditionalDocuments = rDocs.AdditionalDocuments.Select(x => (ApplicationDocument)x).ToList(),
        };

        var setDocumentsResult = application.SetDocuments(documentRecord);
        if (setDocumentsResult.IsFailed)
            return setDocumentsResult;

        var response = new ApplicationFormMetadataDto
        {
            PrettyId = application.ApplicationNumber, Id = application.Id,
        };

        return Result.Ok(response);
    }
}
