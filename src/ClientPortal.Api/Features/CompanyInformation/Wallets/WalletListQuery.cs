using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Base;

namespace SmartCoinOS.ClientPortal.Api.Features.CompanyInformation.Wallets;

internal sealed record WalletListQuery : IQuery<InfoList<WalletListResponse>>
{
    public required UserInfoDto UserInfo { get; init; }
}
