using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.FdtAccounts;

public sealed record SetFdtAccountsFormCommand : ICommand<ApplicationFormMetadataDto>
{
    [FromRoute(Name = "applicationId")]
    public required ApplicationId ApplicationId { get; init; }

    [FromBody]
    public required List<FdtAccountRequest> FdtAccounts { get; init; } = [];

    public required UserInfoDto UserInfo { get; init; }
}

public sealed record FdtAccountRequest
{
    public required string ClientName { get; init; }
    public required string AccountNumber { get; init; }
    public DateTimeOffset? CreatedAt { get; init; }
}
