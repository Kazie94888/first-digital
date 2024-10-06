using SmartCoinOS.ClientPortal.Api.Base;
using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.ClientPortal.Api.Extensions;

internal static class HttpContextExtensions
{
    public static UserInfoDto GetUserInfo(this HttpContext context)
    {
        var userIdClaim = context.User.FindFirst(ApplicationConstants.Claims.UserId)?.Value;
        var clientIdClaim = context.User.FindFirst(ApplicationConstants.Claims.ClientId)?.Value;
        var username = context.User.Identity?.Name;

        if (userIdClaim is null || clientIdClaim is null || username is null)
            throw new NotSupportedException("User identity could not be formed");

        return new UserInfoDto
        {
            Id = new Guid(userIdClaim),
            Username = username,
            ClientId = ClientId.Parse(clientIdClaim)
        };
    }
}