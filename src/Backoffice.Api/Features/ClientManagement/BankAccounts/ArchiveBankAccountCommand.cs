using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.BankAccounts;

public sealed record ArchiveBankAccountCommand : ICommand<EntityId>
{
    [FromRoute(Name = "clientId")]
    public ClientId ClientId { get; init; }
    [FromRoute(Name = "bankAccountId")]
    public BankAccountId BankAccountId { get; init; }
    public required UserInfoDto UserInfo { get; init; }
}
