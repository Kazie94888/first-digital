using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.AuthorizedUsers;

public class ChangeAuthorizedUserStatusCommandHandler : ICommandHandler<ChangeAuthorizedUserStatusCommand, EntityId>
{
    private readonly DataContext _context;

    public ChangeAuthorizedUserStatusCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityId>> Handle(ChangeAuthorizedUserStatusCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FirstAsync(x => x.Id == request.ClientId, cancellationToken);

        var result = request.Status switch
        {
            AuthorizedUserStatus.Suspended => client.SuspendAuthorizedUser(request.UserId, request.UserInfo),
            AuthorizedUserStatus.Active => client.ReactiveAuthorizedUser(request.UserId, request.UserInfo),
            _ => throw new ArgumentException("Invalid status was provided")
        };

        if (result.IsFailed)
            return result;

        return Result.Ok(new EntityId(request.UserId.Value));
    }
}