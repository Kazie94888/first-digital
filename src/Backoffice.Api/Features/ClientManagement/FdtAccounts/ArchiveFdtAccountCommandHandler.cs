using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.FdtAccounts;

internal sealed class ArchiveFdtAccountCommandHandler : ICommandHandler<ArchiveFdtAccountCommand, EntityId>
{
    private readonly DataContext _context;

    public ArchiveFdtAccountCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityId>> Handle(ArchiveFdtAccountCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FirstAsync(x => x.Id == request.ClientId, cancellationToken);

        var archiveResult = client.ArchiveFdtAccount(request.FdtAccountId, request.UserInfo);

        if (archiveResult.IsFailed)
            return Result.Fail(archiveResult.Errors);

        return Result.Ok(new EntityId(request.FdtAccountId.Value));
    }
}