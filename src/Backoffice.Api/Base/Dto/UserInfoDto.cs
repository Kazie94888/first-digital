using SmartCoinOS.Domain.Enums;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Backoffice.Api.Base.Dto;

public sealed class UserInfoDto
{
    public required Guid Id { get; init; }
    public required string Username { get; init; }
    public required UserInfoType Type { get; init; }

    public static implicit operator UserInfoDto(UserInfo userInfo) => new()
    {
        Id = userInfo.Id,
        Username = userInfo.Username,
        Type = userInfo.Type
    };

    public static implicit operator UserInfo(UserInfoDto dto) => new()
    {
        Id = dto.Id,
        Username = dto.Username,
        Type = dto.Type
    };

    public static ValueTask<UserInfoDto> BindAsync(HttpContext context)
    {
        return ValueTask.FromResult(context.GetUserInfo());
    }
}