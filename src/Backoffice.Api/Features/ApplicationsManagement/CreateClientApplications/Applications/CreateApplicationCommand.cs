using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.Applications;

public sealed record CreateApplicationCommand : ICommand<ApplicationFormMetadataDto>
{
    [FromBody]
    public required CreateApplication Application { get; init; }

    public required UserInfoDto UserInfo { get; init; }
}

public sealed record CreateApplication
{
    public required string LegalEntityName { get; init; }
}
