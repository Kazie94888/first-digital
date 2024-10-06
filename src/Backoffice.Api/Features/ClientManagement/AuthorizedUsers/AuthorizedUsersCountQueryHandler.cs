using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.AuthorizedUsers;

internal sealed class AuthorizedUsersCountQueryHandler : IQueryHandler<AuthorizedUsersCountQuery, AuthorizedUsersCountResponse>
{
    private readonly ReadOnlyDataContext _context;

    public AuthorizedUsersCountQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<AuthorizedUsersCountResponse>> Handle(AuthorizedUsersCountQuery request, CancellationToken cancellationToken)
    {
        var aggregatedCounts = await _context.AuthorizedUsers
            .Where(user => user.ClientId == request.ClientId)
            .GroupBy(x => x.Status)
            .Select(x => new
            {
                Status = x.Key,
                Count = x.Count()
            })
            .ToListAsync(cancellationToken);

        return Result.Ok(new AuthorizedUsersCountResponse
        {
            Active = aggregatedCounts.Where(x => x.Status != AuthorizedUserStatus.Archived).Sum(x => x.Count),
            Archived = aggregatedCounts.Where(x => x.Status == AuthorizedUserStatus.Archived).Sum(x => x.Count),
        });
    }
}