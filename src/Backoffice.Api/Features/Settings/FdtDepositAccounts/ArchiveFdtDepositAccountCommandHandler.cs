using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.Settings.FdtDepositAccounts;

internal sealed class ArchiveFdtDepositAccountCommandHandler : ICommandHandler<ArchiveFdtDepositAccountCommand, EntityId>
{
    private readonly DataContext _context;

    public ArchiveFdtDepositAccountCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityId>> Handle(ArchiveFdtDepositAccountCommand request, CancellationToken cancellationToken)
    {
        var fdtDepositAccount = await _context.FdtDepositAccounts.FirstAsync(x => x.Id == request.Id, cancellationToken);
        var archiveResult = fdtDepositAccount.Archive(request.UserInfo);

        if (archiveResult.IsFailed)
            return Result.Fail(archiveResult.Errors);

        return Result.Ok(new EntityId(fdtDepositAccount.Id.Value));
    }
}
