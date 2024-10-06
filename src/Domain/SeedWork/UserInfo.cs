using SmartCoinOS.Domain.Enums;

namespace SmartCoinOS.Domain.SeedWork;

public sealed class UserInfo
{
    public required Guid Id { get; init; }
    public required string Username { get; init; }
    public required UserInfoType Type { get; init; }
}