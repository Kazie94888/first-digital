using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Extensions.Fiql;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.Applications;

public sealed class ApplicationListQueryHandler : IQueryHandler<ApplicationListQuery, InfoPagedList<GetApplicationsResponse>>
{
    private readonly ReadOnlyDataContext _context;

    public ApplicationListQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<InfoPagedList<GetApplicationsResponse>>> Handle(ApplicationListQuery request, CancellationToken cancellationToken)
    {
        var dataQuery = (from a in _context.Applications
                         where a.Archived == request.Archived
                         select new GetApplicationsResponse
                         {
                             Id = a.Id,
                             LegalEntityName = a.LegalEntityName,
                             CreatedAt = a.CreatedAt,
                             CreatedBy = a.CreatedBy,
                             PrettyId = a.ApplicationNumber,
                             Status = a.Status
                         }).Filter(request);

        var totalCount = await dataQuery.CountAsync(cancellationToken);
        var applications = await dataQuery.SortAndPage(request).ToListAsync(cancellationToken);
        var applicationList = new InfoPagedList<GetApplicationsResponse>(applications, totalCount, request.Page, request.Take);

        await AppendCountByStateAsync(request, applicationList, cancellationToken);

        return Result.Ok(applicationList);
    }

    private async Task AppendCountByStateAsync(ApplicationListQuery request, InfoPagedList<GetApplicationsResponse> applicationList, CancellationToken cancellationToken)
    {
        var countQuery = await (from a in _context.Applications.Filter(request)
                                group a by a.Archived into cGr
                                select new
                                {
                                    Archived = cGr.Key,
                                    Count = cGr.Count()
                                }).ToListAsync(cancellationToken);

        var archivedCount = countQuery.Where(x => x.Archived).Select(x => x.Count).Sum();
        var activeCount = countQuery.Where(x => !x.Archived).Select(x => x.Count).Sum();

        applicationList.AddInfo("countActive", activeCount);
        applicationList.AddInfo("countArchived", archivedCount);
    }
}
