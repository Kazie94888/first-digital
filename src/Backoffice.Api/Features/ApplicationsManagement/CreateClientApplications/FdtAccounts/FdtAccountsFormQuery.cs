﻿using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.FdtAccounts;

public sealed class FdtAccountsFormQuery : IQuery<ApplicationFormDto>
{
    [FromRoute(Name = "applicationId")]
    public required ApplicationId ApplicationId { get; init; }
}
