using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.ClientPortal.Api.Features.CompanyInformation.FdtAccounts;

internal sealed record FdtAccountDetailsQuery : IQuery<FdtAccountDetailsResponse>
{
    [FromRoute(Name = "fdtAccountId")]
    public required FdtAccountId FdtAccountId { get; init; }

    public required UserInfoDto UserInfo { get; init; }
}
