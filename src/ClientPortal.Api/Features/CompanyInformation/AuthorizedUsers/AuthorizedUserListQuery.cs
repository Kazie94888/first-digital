using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.ClientPortal.Api.Application;
using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Base;

namespace SmartCoinOS.ClientPortal.Api.Features.CompanyInformation.AuthorizedUsers;

internal sealed record AuthorizedUserListQuery : PagedListQuery, IQuery<InfoPagedList<AuthorizedUserListResponse>>
{
    public required UserInfoDto UserInfo { get; init; }
}
