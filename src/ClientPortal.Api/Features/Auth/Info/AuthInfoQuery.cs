using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.ClientPortal.Api.Features.Auth.Info;

internal sealed record AuthInfoQuery : IQuery<AuthInfoResponse>
{
    public required UserInfoDto UserInfo { get; init; }
}

public sealed record AuthInfoResponse
{
    public required UserAuthInfoResponse User { get; init; }
    public required ClientAuthInfoResponse Client { get; init; }
}

public sealed record UserAuthInfoResponse
{
    public required AuthorizedUserId Id { get; init; }
    public required string ExternalId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public AuthorizedUserStatus Status { get; init; }
    public DateTimeOffset? ArchivedAt { get; init; }
}

public sealed record ClientAuthInfoResponse
{
    public required ClientId ClientId { get; init; }
    public required string LegalEntityName { get; init; }
    public required ClientStatus Status { get; init; }
}
