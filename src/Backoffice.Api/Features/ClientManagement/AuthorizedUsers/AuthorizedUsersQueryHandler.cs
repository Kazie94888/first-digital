using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Extensions.Fiql;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.AuthorizedUsers;

internal sealed class AuthorizedUsersQueryHandler : IQueryHandler<AuthorizedUsersQuery, InfoPagedList<AuthorizedUsersResponse>>
{
    private readonly ReadOnlyDataContext _context;

    public AuthorizedUsersQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<InfoPagedList<AuthorizedUsersResponse>>> Handle(AuthorizedUsersQuery request, CancellationToken cancellationToken)
    {
        var usersQuery = from user in _context.AuthorizedUsers
                         where user.ClientId == request.ClientId
                         select new AuthorizedUsersResponse
                         {
                             Id = user.Id,
                             FirstName = user.FirstName,
                             LastName = user.LastName,
                             Email = user.Email,
                             CreatedAt = user.CreatedAt,
                             Status = user.Status
                         };

        if (request.OnlyArchived)
            usersQuery = usersQuery.Where(x => x.Status == AuthorizedUserStatus.Archived);
        else
            usersQuery = usersQuery.Where(x => x.Status != AuthorizedUserStatus.Archived);

        var totalCount = await usersQuery.CountAsync(cancellationToken);

        var users = await usersQuery.SortAndPage(request).ToListAsync(cancellationToken);

        return Result.Ok(new InfoPagedList<AuthorizedUsersResponse>(users, totalCount, request.Page, request.Take));
    }
}