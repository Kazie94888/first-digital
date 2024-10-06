using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.EntityParticulars;

internal sealed class DeclineEntityParticularCommandHandler : ICommandHandler<DeclineEntityParticularCommand, EntityId>
{
    private readonly DataContext _context;

    public DeclineEntityParticularCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityId>> Handle(DeclineEntityParticularCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FirstAsync(x => x.Id == request.ClientId, cancellationToken);

        var declineResult = client.DeclineEntityParticular(request.EntityParticularId, request.UserInfo);
        if (declineResult.IsFailed)
            return declineResult;

        return Result.Ok(new EntityId(request.EntityParticularId.Value));
    }
}