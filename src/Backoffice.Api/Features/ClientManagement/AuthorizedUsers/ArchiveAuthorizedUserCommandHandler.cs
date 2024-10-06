using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.AuthorizedUsers;

public sealed class ArchiveAuthorizedUserCommandHandler : ICommandHandler<ArchiveAuthorizedUserCommand, EntityId>
{
    private readonly DataContext _context;

    public ArchiveAuthorizedUserCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityId>> Handle(ArchiveAuthorizedUserCommand command,
        CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FirstAsync(client => client.Id == command.ClientId, cancellationToken);

        var archiveResult = client.ArchiveAuthorizedUser(command.UserId, command.UserInfo);

        if (archiveResult.IsFailed)
            return archiveResult;

        return Result.Ok(new EntityId(command.UserId.Value));
    }
}
