using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.FdtAccounts;

internal sealed class DeclineFdtAccountCommandHandler : ICommandHandler<DeclineFdtAccountCommand, EntityId>
{
    private readonly DataContext _context;

    public DeclineFdtAccountCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityId>> Handle(DeclineFdtAccountCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FirstAsync(x => x.Id == request.ClientId, cancellationToken);

        var declineFdtAccountResult = client.DeclineFdtAccount(request.FdtAccountId, request.UserInfo);
        if (declineFdtAccountResult.IsFailed)
            return declineFdtAccountResult;

        return Result.Ok(new EntityId(request.FdtAccountId.Value));
    }
}