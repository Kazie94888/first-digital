using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.BankAccounts;

internal sealed class DeclineBankAccountCommandHandler : ICommandHandler<DeclineBankAccountCommand, EntityId>
{
    private readonly DataContext _context;

    public DeclineBankAccountCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityId>> Handle(DeclineBankAccountCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FirstAsync(x => x.Id == request.ClientId, cancellationToken);

        var declineResult = client.DeclineBankAccount(request.BankAccountId, request.UserInfo);
        if (declineResult.IsFailed)
            return declineResult;

        return Result.Ok(new EntityId(request.BankAccountId.Value));
    }
}