using MediatR;
using SmartCoinOS.Extensions;

namespace SmartCoinOS.Backoffice.Api.Features.FileStorage;

internal class EndpointBuilder : IEndpointBuilder
{
    public void Map(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost("/file-storage/upload-temp", async (
            IMediator mediator,
            [AsParameters] UploadFileCommand uploadFileCommand,
            CancellationToken cancellationToken) =>
        {
            var uploadFileResult = await mediator.Send(uploadFileCommand, cancellationToken);
            return uploadFileResult.ToHttpResult();
        }).DisableAntiforgery();

        routeBuilder.MapPost("/file-storage/delete", async (
            IMediator mediator,
            [AsParameters] DeleteFileCommand deleteFileCommand,
            CancellationToken cancellationToken
            ) =>
        {
            var deleteResult = await mediator.Send(deleteFileCommand, cancellationToken);
            return deleteResult.ToHttpResult();
        });

        routeBuilder.MapPost("/file-storage/download/{documentId}", async (
            IMediator mediator,
            [AsParameters] FileStorageDownloadQuery query,
            CancellationToken cancellationToken
            ) =>
        {
            var downloadResult = await mediator.Send(query, cancellationToken);
            return downloadResult.ToHttpResult();
        });

        routeBuilder.MapGet("/file-storage/document-types", async (
            IMediator mediator,
            [AsParameters] DocumentTypeListQuery query,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return result.ToHttpResult();
        });
    }
}
