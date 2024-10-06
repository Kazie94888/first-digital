using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Persistence;

namespace SmartCoinOS.ClientPortal.Api.Features.CompanyInformation.Documents;

internal class CreateCompanyDocumentCommandHandler : ICommandHandler<CreateCompanyDocumentCommand, EntityId>
{
    private readonly DataContext _context;

    public CreateCompanyDocumentCommandHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<Result<EntityId>> Handle(CreateCompanyDocumentCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FirstAsync(x => x.Id == request.UserInfo.ClientId, cancellationToken);
        client.AddDocument(request.DocumentId, request.Document.FileName, request.Document.DocumentType, request.UserInfo);
        _context.Update(client);

        var document = await _context.Documents.FirstAsync(d => d.Id == request.DocumentId, cancellationToken: cancellationToken);
        document.ConnectClient(client.Id, request.Document.DocumentType);
        _context.Update(document);

        return Result.Ok(new EntityId(client.Id.Value));
    }
}
