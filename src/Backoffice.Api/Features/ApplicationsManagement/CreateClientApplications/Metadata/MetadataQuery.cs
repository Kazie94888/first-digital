using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Application.Enums;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.Metadata;

public sealed record MetadataQuery : IQuery<MetadataQueryResponse>
{
    [FromRoute(Name = "applicationId")]
    public required ApplicationId ApplicationId { get; init; }
}

public sealed record MetadataQueryResponse
{
    public required ApplicationId Id { get; init; }
    public required string PrettyId { get; init; }
    public required string LegalEntityName { get; init; }
    public required ApplicationStatus Status { get; init; }
    public required Dictionary<ApplicationFormType, MetadataRecord> Steps { get; init; } = [];
    public required UserInfoDto CreatedBy { get; init; }
    public required DateTimeOffset CreatedDate { get; init; }
    public required bool IsArchived { get; init; }
}

public sealed record MetadataRecord
{
    public required bool Completed { get; init; }
}
