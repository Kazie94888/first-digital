using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Base;

namespace SmartCoinOS.ClientPortal.Api.Features.CompanyInformation.FdtAccounts;

internal sealed record FdtAccountListQuery : IQuery<InfoList<FdtAccountListResponse>>
{
    public required UserInfoDto UserInfo { get; init; }
}
