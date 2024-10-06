using MediatR;
using SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.ReviewClientApplications.Applications;
using SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.ReviewClientApplications.Sections;
using SmartCoinOS.Extensions;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.ReviewClientApplications;

internal class EndpointBuilder : IEndpointBuilder
{
    public void Map(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPut("/applications/{applicationId}/verify", async (
            IMediator mediator,
            [AsParameters] VerifySectionCommand verifySectionCommand,
            CancellationToken cancellationToken
        ) =>
        {
            var verificationResult = await mediator.Send(verifySectionCommand, cancellationToken);
            return verificationResult.ToHttpResult();
        });

        routeBuilder.MapPut("/applications/{applicationId}/additional-info-required", async (
            IMediator mediator,
            [AsParameters] RequireAdditionalInfoCommand requireAdditionalInfoCommand,
            CancellationToken cancellationToken
        ) =>
        {
            var aditionalInfoResult = await mediator.Send(requireAdditionalInfoCommand, cancellationToken);
            return aditionalInfoResult.ToHttpResult();
        });

        routeBuilder.MapPut("/applications/{applicationId}/reject", async (
            IMediator mediator,
            [AsParameters] RejectApplicationCommand rejectApplicationCommand,
            CancellationToken cancellationToken
        ) =>
        {
            var rejectResult = await mediator.Send(rejectApplicationCommand, cancellationToken);
            return rejectResult.ToHttpResult();
        });

        routeBuilder.MapPut("/applications/{applicationId}/approve", async (
            IMediator mediator,
            [AsParameters] ApproveApplicationCommand approveApplicationCommand,
            CancellationToken cancellationToken
        ) =>
        {
            var approveResult = await mediator.Send(approveApplicationCommand, cancellationToken);
            return approveResult.ToHttpResult();
        });


        routeBuilder.MapPut("/applications/{applicationId}/archive", async (
            IMediator mediator,
            [AsParameters] ArchiveApplicationCommand archiveApplicationCommand,
            CancellationToken cancellationToken
        ) =>
        {
            var archiveResult = await mediator.Send(archiveApplicationCommand, cancellationToken);
            return archiveResult.ToHttpResult();
        });
    }
}