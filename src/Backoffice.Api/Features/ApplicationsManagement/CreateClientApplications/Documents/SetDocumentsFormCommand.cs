using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.Documents;

public sealed record SetDocumentsFormCommand : ICommand<ApplicationFormMetadataDto>
{
    [FromRoute(Name = "applicationId")]
    public required ApplicationId ApplicationId { get; init; }

    [FromBody]
    public required SetDocumentsRequest Documents { get; init; }
}

public sealed record SetDocumentsRequest
{
    public required DocumentDto MemorandumOfAssociation { get; init; }
    public required DocumentDto BankStatement { get; init; }
    public required DocumentDto CertificateOfIncumbency { get; init; }
    public required DocumentDto GroupOwnershipAndStructure { get; init; }
    public required DocumentDto CertificateOfIncAndNameChange { get; init; }
    public required DocumentDto RegisterOfShareholder { get; init; }
    public required DocumentDto RegisterOfDirectors { get; init; }
    public required DocumentDto BoardResolutionAppointingAdditionalUsers { get; init; }

    public required List<DocumentDto> AdditionalDocuments { get; init; } = [];

    public DateTimeOffset? CreatedAt { get; init; }
}
