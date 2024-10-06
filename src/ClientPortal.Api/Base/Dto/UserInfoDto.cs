using SmartCoinOS.ClientPortal.Api.Extensions;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Enums;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.ClientPortal.Api.Base.Dto;

internal sealed class UserInfoDto
{
    public required Guid Id { get; init; }
    public required string Username { get; init; }
    public required ClientId ClientId { get; init; }

    public static implicit operator UserInfo(UserInfoDto dto) => new()
    {
        Id = dto.Id,
        Username = dto.Username,
        Type = UserInfoType.ClientPortal
    };

    public static ValueTask<UserInfoDto> BindAsync(HttpContext context)
    {
        return ValueTask.FromResult(context.GetUserInfo());
    }
}
