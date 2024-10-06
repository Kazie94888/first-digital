using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Enums;

namespace SmartCoinOS.Backoffice.Api.Extensions;

public static class HttpContextExtensions
{
    private const string _userIdClaim = "http://schemas.microsoft.com/identity/claims/objectidentifier";

    public static UserInfoDto GetUserInfo(this HttpContext context)
    {
        var userIdClaim = context.User.FindFirst(_userIdClaim)?.Value;
        var username = context.User.Identity?.Name;

        var userId = string.IsNullOrEmpty(userIdClaim) ? Guid.NewGuid() : new Guid(userIdClaim);

        return new UserInfoDto
        {
            Id = userId,
            Type = UserInfoType.BackOffice,
            Username = username ?? "@demoUser"
        };
    }
}