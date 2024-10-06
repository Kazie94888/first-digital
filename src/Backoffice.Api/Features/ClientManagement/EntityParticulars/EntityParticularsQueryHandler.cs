using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.EntityParticulars;

internal sealed class EntityParticularsQueryHandler : IQueryHandler<EntityParticularsQuery, EntityParticularsResponse>
{
    private readonly DataContext _context;

    public EntityParticularsQueryHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityParticularsResponse>> Handle(EntityParticularsQuery command, CancellationToken cancellationToken)
    {
        var client = await _context.Clients
                            .Include(b => b.EntityParticulars.Where(ep => !ep.Archived))
                            .Include(a => a.Address)
                            .Where(x => x.Id == command.ClientId)
                            .FirstAsync(cancellationToken);

        var currentEp = client.CurrentEntityParticular;
        var entityParticulars = new EntityParticularsResponse
        {
            Particulars = new Particulars
            {
                RegistrationNumber = currentEp.RegistrationNumber,
                CountryOfInc = currentEp.CountryOfInc,
                DateOfInc = currentEp.DateOfIncorporation,
                LegalStructure = currentEp.LegalStructure,
                LegalStructureOther = currentEp.StructureDetails
            },
            Address = new Address
            {
                FullAddress = client.Address.GetFullAddress()
            },
            Contacts = new Contacts
            {
                Email = client.Contact.Email,
                Phone = client.Contact.Phone
            }
        };

        return Result.Ok(entityParticulars);
    }
}