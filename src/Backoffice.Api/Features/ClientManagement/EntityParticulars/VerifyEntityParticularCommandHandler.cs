using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.EntityParticulars;

internal sealed class VerifyEntityParticularCommandHandler : ICommandHandler<VerifyEntityParticularCommand, EntityId>
{
    private readonly DataContext _context;

    public VerifyEntityParticularCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityId>> Handle(VerifyEntityParticularCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FirstAsync(c => c.Id == request.ClientId, cancellationToken);

        var verifyResult = client.VerifyEntityParticular(request.EntityParticularId, request.UserInfo);
        if (verifyResult.IsFailed)
            return verifyResult;

        return Result.Ok(new EntityId(request.EntityParticularId.Value));
    }
}