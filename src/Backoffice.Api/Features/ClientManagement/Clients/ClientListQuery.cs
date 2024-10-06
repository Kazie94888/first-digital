using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Application;
using SmartCoinOS.Domain.Base;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Clients;

public sealed record ClientListQuery : PagedListFiqlQuery, IQuery<InfoPagedList<ClientListResponse>>;
