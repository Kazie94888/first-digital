using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Application;
using SmartCoinOS.Domain.Base;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Orders;

public sealed record OrderAuditListQuery : PagedListQuery, IQuery<InfoPagedList<OrderAuditListResponse>>
{
}
