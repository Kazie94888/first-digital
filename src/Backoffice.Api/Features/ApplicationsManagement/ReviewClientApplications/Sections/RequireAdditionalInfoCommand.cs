using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Application.Enums;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.ReviewClientApplications.Sections;

public sealed record RequireAdditionalInfoCommand : ICommand<ApplicationFormMetadataDto>
{
    [FromRoute(Name = "applicationId")]
    public required ApplicationId ApplicationId { get; init; }

    [FromBody]
    public required RequireInfoRequest Body { get; init; }

    public required UserInfoDto UserInfo { get; init; }
}

public sealed record RequireInfoRequest
{
    public required ApplicationFormType Form { get; init; }
}