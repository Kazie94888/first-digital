using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Application.Enums;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.ReviewClientApplications.Sections;

public sealed record VerifySectionCommand : ICommand<ApplicationFormMetadataDto>
{
    [FromRoute(Name = "applicationId")]
    public required ApplicationId ApplicationId { get; init; }

    [FromBody]
    public required VerifySectionRequest Request { get; init; }

    public required UserInfoDto UserInfo { get; init; }
}

public sealed record VerifySectionRequest
{
    public required ApplicationFormType Form { get; init; }
    public string? Record { get; init; }
    public string? Id { get; init; }
}