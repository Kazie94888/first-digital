using MediatR;
using SmartCoinOS.Backoffice.Api.Features.Settings.Blockchain;
using SmartCoinOS.Backoffice.Api.Features.Settings.DepositBanks;
using SmartCoinOS.Backoffice.Api.Features.Settings.DepositWallets;
using SmartCoinOS.Backoffice.Api.Features.Settings.FdtDepositAccounts;
using SmartCoinOS.Extensions;

namespace SmartCoinOS.Backoffice.Api.Features.Settings;

internal class EndpointBuilder : IEndpointBuilder
{
    public void Map(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPut("/settings/deposit-banks/{id}/archive", async (
            IMediator mediator,
            [AsParameters] ArchiveDepositBankCommand archiveDepositBankCommand,
            CancellationToken cancellationToken
        ) =>
        {
            var archiveDepositBankCommandResult = await mediator.Send(archiveDepositBankCommand, cancellationToken);
            return archiveDepositBankCommandResult.ToHttpResult();
        });

        routeBuilder.MapPost("/settings/deposit-banks", async (
            IMediator mediator,
            [AsParameters] CreateDepositBankCommand createDepositBankCommand,
            CancellationToken cancellationToken
        ) =>
        {
            var createDepositBankResult = await mediator.Send(createDepositBankCommand, cancellationToken);
            return createDepositBankResult.ToHttpResult();
        });

        routeBuilder.MapPut("/settings/deposit-banks/{id}/default", async (
            IMediator mediator,
            [AsParameters] DefaultDepositBankCommand defaultDepositBankCommand,
            CancellationToken cancellationToken
        ) =>
        {
            var defaultDepositBankCommandResult = await mediator.Send(defaultDepositBankCommand, cancellationToken);
            return defaultDepositBankCommandResult.ToHttpResult();
        });

        routeBuilder.MapGet("/settings/deposit-banks/{id}/details", async (
            IMediator mediator,
            [AsParameters] DepositBankDetailsQuery depositBankDetailsQuery,
            CancellationToken cancellationToken
        ) =>
        {
            var depositBankDetailsQueryResult = await mediator.Send(depositBankDetailsQuery, cancellationToken);
            return depositBankDetailsQueryResult.ToHttpResult();
        });

        routeBuilder.MapGet("/settings/deposit-banks/{id}/clients", async (
            IMediator mediator,
            [AsParameters] DepositBankClientsQuery depositBankClientsQuery,
            CancellationToken cancellationToken
        ) =>
        {
            var depositBankClientsQueryResult = await mediator.Send(depositBankClientsQuery, cancellationToken);
            return depositBankClientsQueryResult.ToHttpResult();
        });

        routeBuilder.MapGet("/settings/deposit-banks", async (
            IMediator mediator,
            [AsParameters] DepositBankListQuery depositBankListQuery,
            CancellationToken cancellationToken
        ) =>
        {
            var depositBankListQueryResult = await mediator.Send(depositBankListQuery, cancellationToken);
            return depositBankListQueryResult.ToHttpResult();
        });

        routeBuilder.MapGet("/settings/deposit-wallets", async (
            IMediator mediator,
            [AsParameters] DepositWalletListQuery depositWalletListQuery,
            CancellationToken cancellationToken
        ) =>
        {
            var depositWalletListResult = await mediator.Send(depositWalletListQuery, cancellationToken);
            return depositWalletListResult.ToHttpResult();
        });
        routeBuilder.MapPut("/settings/fdt-deposit-accounts/{id}/archive", async (
            IMediator mediator,
            [AsParameters] ArchiveFdtDepositAccountCommand archiveFdtDepositAccountCommand,
            CancellationToken cancellationToken
        ) =>
        {
            var archiveFdtDepositAccountResult = await mediator.Send(archiveFdtDepositAccountCommand, cancellationToken);
            return archiveFdtDepositAccountResult.ToHttpResult();
        });

        routeBuilder.MapPost("/settings/fdt-deposit-accounts", async (
            IMediator mediator,
            [AsParameters] CreateFdtDepositAccountCommand createFdtDepositAccountCommand,
            CancellationToken cancellationToken
        ) =>
        {
            var createFdtDepositAccountResult = await mediator.Send(createFdtDepositAccountCommand, cancellationToken);
            return createFdtDepositAccountResult.ToHttpResult();
        });

        routeBuilder.MapGet("/settings/fdt-deposit-accounts/{id}/details", async (
            IMediator mediator,
            [AsParameters] FdtDepositAccountDetailsQuery fdtDepositAccountDetailsQuery,
            CancellationToken cancellationToken
        ) =>
        {
            var fdtDepositAccountDetailsResult = await mediator.Send(fdtDepositAccountDetailsQuery, cancellationToken);
            return fdtDepositAccountDetailsResult.ToHttpResult();
        });

        routeBuilder.MapGet("/settings/fdt-deposit-accounts", async (
            IMediator mediator,
            [AsParameters] FdtDepositAccountListQuery fdtDepositAccountListQuery,
            CancellationToken cancellationToken
        ) =>
        {
            var fdtDepositAccountListResult = await mediator.Send(fdtDepositAccountListQuery, cancellationToken);
            return fdtDepositAccountListResult.ToHttpResult();
        });

        routeBuilder.MapGet("/settings/blockchain", async (
            IMediator mediator,
            [AsParameters] BlockchainSettingsQuery blockchainSettingsQuery,
            CancellationToken cancellationToken
            ) =>
        {
            var blockchainSettings = await mediator.Send(blockchainSettingsQuery, cancellationToken);
            return blockchainSettings.ToHttpResult();
        });
    }
}
