using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.Settings.DepositBanks;

internal sealed class ArchiveDepositBankCommandHandler : ICommandHandler<ArchiveDepositBankCommand, EntityId>
{
    private readonly DataContext _context;

    public ArchiveDepositBankCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityId>> Handle(ArchiveDepositBankCommand request, CancellationToken cancellationToken)
    {
        var depositBank = await _context.DepositBanks.FirstAsync(x => x.Id == request.Id, cancellationToken);

        var archiveResult = depositBank.Archive(request.UserInfo);

        if (archiveResult.IsFailed)
            return archiveResult;

        return Result.Ok(new EntityId(depositBank.Id.Value));
    }
}
