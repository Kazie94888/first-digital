using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Application;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Application.Enums;
using SmartCoinOS.Domain.Base;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.Applications;

public sealed record ApplicationListQuery : PagedListFiqlQuery, IQuery<InfoPagedList<GetApplicationsResponse>>
{
    [FromQuery(Name = "archived")]
    public bool Archived { get; init; }
}

public sealed record GetApplicationsResponse
{
    public required ApplicationId Id { get; init; }
    public required string LegalEntityName { get; init; }
    public required UserInfoDto CreatedBy { get; init; }
    public required string PrettyId { get; init; }
    public required ApplicationStatus Status { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}
