using MediatR;
using SmartCoinOS.Backoffice.Api.Features.OrderManagements.Create;
using SmartCoinOS.Backoffice.Api.Features.OrderManagements.Minting;
using SmartCoinOS.Backoffice.Api.Features.OrderManagements.Orders;
using SmartCoinOS.Backoffice.Api.Features.OrderManagements.Redeeming;
using SmartCoinOS.Extensions;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements;

internal class EndpointBuilder : IEndpointBuilder
{
    public void Map(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("/orders/clients/{clientId}/resources", async (
            IMediator mediator,
            [AsParameters] CreateOrderClientResourcesQuery clientResourcesQuery,
            CancellationToken cancellationToken = default) =>
        {
            var resources = await mediator.Send(clientResourcesQuery, cancellationToken);
            return resources.ToHttpResult();
        });

        routeBuilder.MapGet("/orders/create/clients", async (
            IMediator mediator,
            [AsParameters] CreateOrderClientsListQuery createOrderClientsListQuery,
            CancellationToken cancellationToken = default) =>
        {
            var clients = await mediator.Send(createOrderClientsListQuery, cancellationToken);
            return clients.ToHttpResult();
        });

        routeBuilder.MapPost("/orders/mint", async (
            IMediator mediator,
            [AsParameters] MintOrderCommand mintOrderCommand,
            CancellationToken cancellationToken
            ) =>
        {
            var order = await mediator.Send(mintOrderCommand, cancellationToken);
            return order.ToHttpResult();
        });

        routeBuilder.MapGet("/orders/mint/{orderId}", async (
           IMediator mediator,
           [AsParameters] MintOrderDetailsQuery mintOrderDetailsQuery,
           CancellationToken cancellationToken
           ) =>
           {
               var mintOrderDetailsResult = await mediator.Send(mintOrderDetailsQuery, cancellationToken);
               return mintOrderDetailsResult.ToHttpResult();
           });

        routeBuilder.MapPut("/orders/{orderId}/mint/add-rsn-reference", async (
            IMediator mediator,
            [AsParameters] RsnReferenceAddCommand rsnReferenceAddCommand,
            CancellationToken cancellationToken
            ) =>
        {
            var rsnRefResult = await mediator.Send(rsnReferenceAddCommand, cancellationToken);
            return rsnRefResult.ToHttpResult();
        });

        routeBuilder.MapPut("/orders/{orderId}/mint/add-rsn-amount", async (
            IMediator mediator,
            [AsParameters] RsnAmountAddCommand rsnAmountAddCommand,
            CancellationToken cancellationToken
            ) =>
        {
            var rsnAmountAddResult = await mediator.Send(rsnAmountAddCommand, cancellationToken);
            return rsnAmountAddResult.ToHttpResult();
        });

        routeBuilder.MapPut("/orders/{orderId}/mint/signed", async (
            IMediator mediator,
            [AsParameters] TrackMintOrderSignatureCommand trackSignatureCommand,
            CancellationToken cancellationToken = default) =>
        {
            var trackSignatureResult = await mediator.Send(trackSignatureCommand, cancellationToken);
            return trackSignatureResult.ToHttpResult();
        });

        routeBuilder.MapPut("/orders/{orderId}/mint/executed", async (
            IMediator mediator,
            [AsParameters] MintOrderExecutedCommand command,
            CancellationToken cancellationToken = default) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return result.ToHttpResult();
        });

        routeBuilder.MapPost("/orders/redeem", async (
            IMediator mediator,
            [AsParameters] RedeemOrderCommand redeemOrderCommand,
            CancellationToken cancellationToken
            ) =>
        {
            var order = await mediator.Send(redeemOrderCommand, cancellationToken);
            return order.ToHttpResult();
        });

        routeBuilder.MapGet("/orders/redeem/{orderId}", async (
            IMediator mediator,
            [AsParameters] RedeemOrderDetailsQuery query,
            CancellationToken cancellationToken
        ) =>
        {
            var order = await mediator.Send(query, cancellationToken);
            return order.ToHttpResult();
        });

        routeBuilder.MapPut("/orders/{orderId}/redeem/set-tx-hash", async (
            IMediator mediator,
            [AsParameters] RedeemOrderSetTxHashCommand setTxHashCommand,
            CancellationToken cancellationToken = default) =>
        {
            var setTxHashResult = await mediator.Send(setTxHashCommand, cancellationToken);
            return setTxHashResult.ToHttpResult();
        });

        routeBuilder.MapPut("/orders/{orderId}/redeem/confirm-deposit", async (
            IMediator mediator,
            [AsParameters] RedeemOrderConfirmDepositCommand confirmDepositCommand,
            CancellationToken cancellationToken = default) =>
        {
            var confirmDepositResult = await mediator.Send(confirmDepositCommand, cancellationToken);
            return confirmDepositResult.ToHttpResult();
        });

        routeBuilder.MapPut("/orders/{orderId}/redeem/add-rsn-reference", async (
            IMediator mediator,
            [AsParameters] RedeemOrderAddRsnReferenceCommand addRsnReferenceCommand,
            CancellationToken cancellationToken
            ) =>
        {
            var rsnRefResult = await mediator.Send(addRsnReferenceCommand, cancellationToken);
            return rsnRefResult.ToHttpResult();
        });

        routeBuilder.MapPut("/orders/{orderId}/redeem/complete", async (
                    IMediator mediator,
                    [AsParameters] RedeemOrderCompleteCommand completeCommand,
                    CancellationToken cancellationToken = default) =>
        {
            var completeResult = await mediator.Send(completeCommand, cancellationToken);
            return completeResult.ToHttpResult();
        });

        routeBuilder.MapPut("/orders/{orderId}/redeem/cancel", async (
            IMediator mediator,
            [AsParameters] CancelRedeemOrderCommand command,
            CancellationToken cancellationToken = default) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return result.ToHttpResult();
        });

        routeBuilder.MapPut("/orders/{orderId}/redeem/signed", async (
            IMediator mediator,
            [AsParameters] TrackRedeemOrderSignatureCommand trackSignatureCommand,
            CancellationToken cancellationToken = default) =>
        {
            var trackSignatureResult = await mediator.Send(trackSignatureCommand, cancellationToken);
            return trackSignatureResult.ToHttpResult();
        });

        routeBuilder.MapPut("/orders/{orderId}/redeem/executed", async (
            IMediator mediator,
            [AsParameters] RedeemOrderExecutedCommand command,
            CancellationToken cancellationToken = default) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return result.ToHttpResult();
        });

        routeBuilder.MapGet("/orders", async (
            IMediator mediator,
            [AsParameters] OrderListQuery orderQuery,
            CancellationToken cancellationToken = default) =>
        {
            var orders = await mediator.Send(orderQuery, cancellationToken);
            return orders.ToHttpResult();
        });

        routeBuilder.MapGet("/orders/audits", async (
            IMediator mediator,
            [AsParameters] OrderAuditListQuery getOrderAudits,
            CancellationToken cancellationToken = default) =>
        {
            var audits = await mediator.Send(getOrderAudits, cancellationToken);
            return audits.ToHttpResult();
        });

        routeBuilder.MapGet("/orders/{orderId}/audits", async (
            IMediator mediator,
            [AsParameters] DetailedOrderAuditListQuery auditListQuery,
            CancellationToken cancellationToken = default) =>
        {
            var auditsResult = await mediator.Send(auditListQuery, cancellationToken);
            return auditsResult.ToHttpResult();
        });
    }
}