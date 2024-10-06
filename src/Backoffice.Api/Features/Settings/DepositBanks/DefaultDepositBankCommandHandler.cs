using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.Settings.DepositBanks;

internal sealed class DefaultDepositBankCommandHandler : ICommandHandler<DefaultDepositBankCommand, EntityId>
{
    private readonly DataContext _context;

    public DefaultDepositBankCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityId>> Handle(DefaultDepositBankCommand request, CancellationToken cancellationToken)
    {
        var depositBank = await _context.DepositBanks.FirstAsync(x => x.Id == request.Id, cancellationToken);

        var defaultResult = depositBank.MakeDefault(request.UserInfo);
        if (defaultResult.IsFailed)
            return defaultResult;

        return Result.Ok(new EntityId(depositBank.Id.Value));
    }
}