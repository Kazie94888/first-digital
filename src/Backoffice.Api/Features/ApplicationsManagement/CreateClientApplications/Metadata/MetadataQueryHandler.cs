using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Application.Enums;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.Metadata;

internal sealed class MetadataQueryHandler : IQueryHandler<MetadataQuery, MetadataQueryResponse>
{
    private readonly ReadOnlyDataContext _context;

    public MetadataQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<MetadataQueryResponse>> Handle(MetadataQuery request, CancellationToken cancellationToken)
    {
        var application = await _context.Applications.FirstAsync(x => x.Id == request.ApplicationId, cancellationToken);

        var applicationMetadata = new MetadataQueryResponse
        {
            Id = application.Id,
            LegalEntityName = application.LegalEntityName,
            PrettyId = application.ApplicationNumber.Value,
            Status = application.Status,
            IsArchived = application.Archived,
            CreatedBy = application.CreatedBy,
            CreatedDate = application.CreatedAt,
            Steps = new Dictionary<ApplicationFormType, MetadataRecord>
            {
                {
                    ApplicationFormType.EntityParticulars,
                    new MetadataRecord { Completed = application.EntityParticulars is not null }
                },
                {
                    ApplicationFormType.AuthorizedUsers,
                    new MetadataRecord { Completed = application.AuthorizedUsers.AuthorizedUsers.Count > 0 }
                },
                {
                    ApplicationFormType.BusinessInfo,
                    new MetadataRecord { Completed = application.BusinessInfo is not null }
                },
                {
                    ApplicationFormType.BankAccounts,
                    new MetadataRecord { Completed = application.BankAccounts.BankRecords.Count > 0 }
                },
                {
                    ApplicationFormType.Wallets,
                    new MetadataRecord { Completed = application.Wallets.Wallets.Count > 0 }
                },
                {
                    ApplicationFormType.FdtAccounts,
                    new MetadataRecord { Completed = application.FdtAccounts.FdtAccounts.Count > 0 }
                },
                {
                    ApplicationFormType.Documents,
                    new MetadataRecord { Completed = application.Documents is not null }
                },
            }
        };

        return Result.Ok(applicationMetadata);
    }
}