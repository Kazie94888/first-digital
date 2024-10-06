using MediatR;
using SmartCoinOS.ClientPortal.Api.Extensions;
using SmartCoinOS.ClientPortal.Api.Features.CompanyInformation.AuthorizedUsers;
using SmartCoinOS.ClientPortal.Api.Features.CompanyInformation.BankAccounts;
using SmartCoinOS.ClientPortal.Api.Features.CompanyInformation.Documents;
using SmartCoinOS.ClientPortal.Api.Features.CompanyInformation.EntityParticulars;
using SmartCoinOS.ClientPortal.Api.Features.CompanyInformation.FdtAccounts;
using SmartCoinOS.ClientPortal.Api.Features.CompanyInformation.Wallets;
using SmartCoinOS.Extensions;

namespace SmartCoinOS.ClientPortal.Api.Features.CompanyInformation;

internal sealed class EndpointBuilder : IEndpointBuilder
{
    public void Map(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("/company/entity-particulars", async (
            IMediator mediator,
            [AsParameters] EntityParticularQuery query,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization();

        routeBuilder.MapGet("/company/bank-accounts", async (
            IMediator mediator,
            [AsParameters] BankAccountListQuery query,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization();

        routeBuilder.MapGet("/company/bank-accounts/{bankId}", async (
            IMediator mediator,
            [AsParameters] BankAccountDetailsQuery query,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization();

        routeBuilder.MapGet("/company/wallets", async (
            IMediator mediator,
            [AsParameters] WalletListQuery query,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization();

        routeBuilder.MapGet("/company/wallets/{walletId}", async (
            IMediator mediator,
            [AsParameters] WalletDetailsQuery query,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization();

        routeBuilder.MapGet("/company/fdt-accounts", async (
            IMediator mediator,
            [AsParameters] FdtAccountListQuery query,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization();

        routeBuilder.MapGet("/company/fdt-accounts/{fdtAccountId}", async (
            IMediator mediator,
            [AsParameters] FdtAccountDetailsQuery query,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization();

        routeBuilder.MapGet("/company/authorized-users", async (
            IMediator mediator,
            [AsParameters] AuthorizedUserListQuery query,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization();
        
        routeBuilder.MapGet("/company/documents", async (
            IMediator mediator,
            [AsParameters] DocumentListQuery query,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization();
        
        routeBuilder.MapPut("/company/documents/{documentId}", async (
            IMediator mediator,
            [AsParameters] CreateCompanyDocumentCommand query,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization();
    }
}
