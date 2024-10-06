using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.AuthorizedUsers;

public sealed record ChangeAuthorizedUserStatusCommand : ICommand<EntityId>
{
    public required ClientId ClientId { get; init; }
    public required AuthorizedUserId UserId { get; init; }
    public AuthorizedUserStatus Status { get; init; }
    public required UserInfoDto UserInfo { get; init; }
}
