using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.ClientPortal.Api.Features.CompanyInformation.BankAccounts;

internal sealed record BankAccountDetailsQuery : IQuery<BankAccountDetailsResponse>
{
    [FromRoute(Name = "bankId")]
    public required BankAccountId BankId { get; init; }

    public required UserInfoDto UserInfo { get; init; }
}
