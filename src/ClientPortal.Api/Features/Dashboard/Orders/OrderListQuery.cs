using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.ClientPortal.Api.Application;
using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Base;

namespace SmartCoinOS.ClientPortal.Api.Features.Dashboard.Orders;

internal sealed record OrderListQuery : PagedListFiqlQuery, IQuery<InfoPagedList<OrderListResponse>>
{
    public required UserInfoDto UserInfo { get; init; }
}
