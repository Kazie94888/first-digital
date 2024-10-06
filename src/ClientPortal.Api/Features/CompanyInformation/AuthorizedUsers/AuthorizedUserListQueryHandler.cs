using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Extensions.Fiql;
using SmartCoinOS.Persistence;

namespace SmartCoinOS.ClientPortal.Api.Features.CompanyInformation.AuthorizedUsers;

internal sealed class
    AuthorizedUserListQueryHandler : IQueryHandler<AuthorizedUserListQuery, InfoPagedList<AuthorizedUserListResponse>>
{
    private readonly ReadOnlyDataContext _context;

    public AuthorizedUserListQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<InfoPagedList<AuthorizedUserListResponse>>> Handle(AuthorizedUserListQuery request,
        CancellationToken cancellationToken)
    {
        var clientId = request.UserInfo.ClientId;

        var usersQuery = from user in _context.AuthorizedUsers
            where user.ClientId == clientId
            select new AuthorizedUserListResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Status = user.Status,
                ExternalId = user.ExternalId
            };

        var totalCount = await usersQuery.CountAsync(cancellationToken);

        var users = await usersQuery.SortAndPage(request).ToListAsync(cancellationToken);

        return Result.Ok(new InfoPagedList<AuthorizedUserListResponse>(users, totalCount, request.Page, request.Take));
    }
}
