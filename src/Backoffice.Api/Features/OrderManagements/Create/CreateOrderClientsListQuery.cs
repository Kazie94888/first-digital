using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Base;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Create;

internal sealed record CreateOrderClientsListQuery : IQuery<InfoList<CreateOrderClientsResponse>>;
