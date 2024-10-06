using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.BankAccounts;

internal sealed class VerifyBankAccountCommandHandler : ICommandHandler<VerifyBankAccountCommand, EntityId>
{
    private readonly DataContext _context;

    public VerifyBankAccountCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityId>> Handle(VerifyBankAccountCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FirstAsync(x => x.Id == request.ClientId, cancellationToken);

        var verifyResult = client.VerifyBankAccount(request.BankAccountId, request.UserInfo);
        if (verifyResult.IsFailed)
            return verifyResult;

        return Result.Ok(new EntityId(request.BankAccountId.Value));
    }
}