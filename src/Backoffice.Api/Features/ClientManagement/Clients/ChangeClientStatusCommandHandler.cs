using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Clients;

public class ChangeClientStatusCommandHandler : ICommandHandler<ChangeClientStatusCommand, EntityId>
{
    private readonly DataContext _context;

    public ChangeClientStatusCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityId>> Handle(ChangeClientStatusCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FirstAsync(x => x.Id == request.ClientId, cancellationToken);

        var statusResult = request.Status switch
        {
            ClientStatus.Active => client.Activate(request.UserInfo),
            ClientStatus.Dormant => client.Dormant(request.UserInfo),
            ClientStatus.TemporarlySuspended => client.Suspend(permanent: false, request.UserInfo),
            ClientStatus.Inactive => client.Suspend(permanent: true, request.UserInfo),
            _ => throw new ArgumentException("Not supported status was provided"),
        };

        if (statusResult.IsFailed)
            return statusResult;

        return new EntityId(request.ClientId.Value);
    }
}