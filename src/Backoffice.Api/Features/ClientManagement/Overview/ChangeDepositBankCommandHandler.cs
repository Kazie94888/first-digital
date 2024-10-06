using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Overview;

internal sealed class ChangeDepositBankCommandHandler : ICommandHandler<ChangeDepositBankCommand, ChangeDepositBankResponse>
{
    private readonly DataContext _context;

    public ChangeDepositBankCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<ChangeDepositBankResponse>> Handle(ChangeDepositBankCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FirstAsync(x => x.Id == request.ClientId, cancellationToken);
        var depositBank = await _context.DepositBanks.FirstAsync(x => x.Id == request.DepositBankId, cancellationToken);

        client.UpdateDepositBank(request.DepositBankId, request.UserInfo);

        var defaultBank = new ChangeDepositBankResponse
        {
            Id = depositBank.Id,
            BankName = depositBank.Name,
            Iban = depositBank.Iban,
        };

        return Result.Ok(defaultBank);
    }
}