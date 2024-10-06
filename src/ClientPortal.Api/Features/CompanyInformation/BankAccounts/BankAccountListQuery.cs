using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Base;

namespace SmartCoinOS.ClientPortal.Api.Features.CompanyInformation.BankAccounts;

internal sealed record BankAccountListQuery : IQuery<InfoList<BankAccountListResponse>>
{
    public required UserInfoDto UserInfo { get; init; }
}
