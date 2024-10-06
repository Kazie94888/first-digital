using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.BankAccounts;

internal sealed class ArchiveBankAccountCommandHandler : ICommandHandler<ArchiveBankAccountCommand, EntityId>
{
    private readonly DataContext _context;

    public ArchiveBankAccountCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityId>> Handle(ArchiveBankAccountCommand request, CancellationToken cancellationToken)
    {
        var hasActiveOrders = false;
        // remaining: operation should only continue if there's no order in progress using this bank account

        var client = await _context.Clients.FirstAsync(x => x.Id == request.ClientId, cancellationToken);

        var archiveResult = client.ArchiveBankAccount(request.BankAccountId, hasActiveOrders, request.UserInfo);
        if (archiveResult.IsFailed)
            return archiveResult;

        return Result.Ok(new EntityId(request.BankAccountId.Value));
    }
}