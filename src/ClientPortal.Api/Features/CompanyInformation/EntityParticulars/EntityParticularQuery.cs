using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.ClientPortal.Api.Base.Dto;

namespace SmartCoinOS.ClientPortal.Api.Features.CompanyInformation.EntityParticulars;

internal sealed record EntityParticularQuery : IQuery<EntityParticularsResponse>
{
    public required UserInfoDto UserInfo { get; init; }
}
