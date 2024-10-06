using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Clients;

internal sealed class ClientGeneralInfoQueryHandler : IQueryHandler<ClientGeneralInfoQuery, ClientGeneralInfoResponse>
{
    private readonly ReadOnlyDataContext _context;

    public ClientGeneralInfoQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<ClientGeneralInfoResponse>> Handle(ClientGeneralInfoQuery request, CancellationToken cancellationToken)
    {
        var client = await (from c in _context.Clients.Include(b => b.EntityParticulars.Where(ep => !ep.Archived))
                            where c.Id == request.ClientId
                            select c).FirstAsync(cancellationToken);

        return Result.Ok(new ClientGeneralInfoResponse
        {
            Id = client.Id.Value,
            ClientName = client.CurrentEntityParticular.LegalEntityName,
            Status = client.Status
        });
    }
}