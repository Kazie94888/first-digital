using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Application;
using SmartCoinOS.Domain.Base;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Orders;

internal sealed record OrderListQuery : PagedListFiqlQuery, IQuery<InfoPagedList<OrderListResponse>>;
