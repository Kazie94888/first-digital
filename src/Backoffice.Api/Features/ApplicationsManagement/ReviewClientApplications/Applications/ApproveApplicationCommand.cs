﻿using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.ReviewClientApplications.Applications;

internal sealed record ApproveApplicationCommand : ICommand<ApplicationFormMetadataDto>
{
    [FromRoute(Name = "applicationId")]
    public required ApplicationId ApplicationId { get; init; }

    public required UserInfoDto UserInfo { get; init; }
}