using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Persistence;

namespace SmartCoinOS.ClientPortal.Api.Features.CompanyInformation.EntityParticulars;

internal sealed class EntityParticularQueryHandler : IQueryHandler<EntityParticularQuery, EntityParticularsResponse>
{
    private readonly ReadOnlyDataContext _context;

    public EntityParticularQueryHandler(ReadOnlyDataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityParticularsResponse>> Handle(EntityParticularQuery request,
        CancellationToken cancellationToken)
    {
        var clientId = request.UserInfo.ClientId;
        var client = await _context.Clients
            .Include(ep => ep.EntityParticulars.Where(e => !e.Archived))
            .Include(a => a.Address)
            .Where(x => x.Id == clientId)
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
            Address = new Address { FullAddress = client.Address.GetFullAddress() },
            Contacts = new Contacts
            {
                Email = client.Contact.Email,
                Phone = client.Contact.Phone
            }
        };

        return Result.Ok(entityParticulars);
    }
}
