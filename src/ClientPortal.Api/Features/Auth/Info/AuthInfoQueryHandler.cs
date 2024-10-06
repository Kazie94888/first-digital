using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Persistence;

namespace SmartCoinOS.ClientPortal.Api.Features.Auth.Info;

internal sealed class AuthInfoQueryHandler : IQueryHandler<AuthInfoQuery, AuthInfoResponse>
{
    readonly ReadOnlyDataContext _context;

    public AuthInfoQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<AuthInfoResponse>> Handle(AuthInfoQuery request, CancellationToken cancellationToken)
    {
        var externalId = request.UserInfo.Id.ToString();
        var clientId = request.UserInfo.ClientId;

        var userInfo = await (from user in _context.AuthorizedUsers
                              join client in _context.Clients
                                                        .Include(b => b.EntityParticulars.Where(ep => !ep.Archived)) on user.ClientId equals client.Id
                              where user.ExternalId == externalId
                                    && user.ClientId == clientId
                              select new AuthInfoResponse
                              {
                                  User = new UserAuthInfoResponse
                                  {
                                      Id = user.Id,
                                      Email = user.Email,
                                      ExternalId = externalId,
                                      FirstName = user.FirstName,
                                      LastName = user.LastName,
                                      Status = user.Status,
                                      ArchivedAt = user.ArchivedAt,
                                  },
                                  Client = new ClientAuthInfoResponse
                                  {
                                      ClientId = client.Id,
                                      LegalEntityName = client.CurrentEntityParticular.LegalEntityName,
                                      Status = client.Status
                                  }
                              }).FirstAsync(cancellationToken);

        return Result.Ok(userInfo);
    }
}