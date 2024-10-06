using MediatR;
using SmartCoinOS.ClientPortal.Api.Extensions;
using SmartCoinOS.Extensions;

namespace SmartCoinOS.ClientPortal.Api.Features.FileStorage;

internal class EndpointBuilder : IEndpointBuilder
{
    public void Map(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost("/file-storage/upload-temp", async (
            IMediator mediator,
            [AsParameters] UploadDocumentCommand command,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return result.ToHttpResult();
        }).DisableAntiforgery().RequireAuthorization();

        routeBuilder.MapGet("/file-storage/documents/{documentId}", async (
            IMediator mediator,
            [AsParameters] DownloadDocumentQuery query,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization();

        routeBuilder.MapPost("/file-storage/delete", async (
            IMediator mediator,
            [AsParameters] DeleteDocumentCommand command,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization();

        routeBuilder.MapGet("/file-storage/document-types", async (
            IMediator mediator,
            [AsParameters] DocumentTypeListQuery query,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization();
    }
}
