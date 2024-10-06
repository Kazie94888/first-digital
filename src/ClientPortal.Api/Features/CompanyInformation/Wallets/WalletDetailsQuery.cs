using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.ClientPortal.Api.Features.CompanyInformation.Wallets;

internal sealed record WalletDetailsQuery : IQuery<WalletDetailsResponse>
{
    [FromRoute(Name = "walletId")] public required WalletId WalletId { get; init; }
    public required UserInfoDto UserInfo { get; init; }
}
