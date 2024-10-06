﻿using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.ReviewClientApplications.Applications;

public sealed record RejectApplicationCommand : ICommand<ApplicationFormMetadataDto>
{
    [FromRoute(Name = "applicationId")]
    public required ApplicationId ApplicationId { get; init; }

    public required UserInfoDto UserInfo { get; init; }
}