using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Backoffice.Api.Features.ClientManagement.Audits;
using SmartCoinOS.Backoffice.Api.Features.ClientManagement.AuthorizedUsers;
using SmartCoinOS.Backoffice.Api.Features.ClientManagement.BankAccounts;
using SmartCoinOS.Backoffice.Api.Features.ClientManagement.Clients;
using SmartCoinOS.Backoffice.Api.Features.ClientManagement.Compliance;
using SmartCoinOS.Backoffice.Api.Features.ClientManagement.EntityParticulars;
using SmartCoinOS.Backoffice.Api.Features.ClientManagement.FdtAccounts;
using SmartCoinOS.Backoffice.Api.Features.ClientManagement.Orders;
using SmartCoinOS.Backoffice.Api.Features.ClientManagement.Overview;
using SmartCoinOS.Backoffice.Api.Features.ClientManagement.Wallets;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Extensions;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement;

internal class EndpointBuilder : IEndpointBuilder
{
    public void Map(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("/clients/{clientId}/audits/overview", async (
            IMediator mediator,
            [AsParameters] ClientAuditListOverviewQuery auditsQuery,
            CancellationToken cancellationToken = default) =>
        {
            var clientAudits = await mediator.Send(auditsQuery, cancellationToken);
            return clientAudits.ToHttpResult();
        });

        routeBuilder.MapGet("/clients/{clientId}/entity-particulars", async (
            IMediator mediator,
            [AsParameters] EntityParticularsQuery entityPartQuery,
            CancellationToken cancellationToken
            ) =>
        {
            var particularsResponse = await mediator.Send(entityPartQuery, cancellationToken);
            return particularsResponse.ToHttpResult();
        });

        routeBuilder.MapPut("/clients/{clientId}/entity-particulars/{entityParticularId}/verify", async (
            IMediator mediator,
            [AsParameters] VerifyEntityParticularCommand command,
            CancellationToken cancellationToken
        ) =>
        {
            var particularsResponse = await mediator.Send(command, cancellationToken);
            return particularsResponse.ToHttpResult();
        });

        routeBuilder.MapPut("/clients/{clientId}/entity-particulars/{entityParticularId}/decline", async (
            IMediator mediator,
            [AsParameters] DeclineEntityParticularCommand command,
            CancellationToken cancellationToken
        ) =>
        {
            var particularsResponse = await mediator.Send(command, cancellationToken);
            return particularsResponse.ToHttpResult();
        });

        routeBuilder.MapGet("/clients/{clientId}/audits", async (
            IMediator mediator,
            [AsParameters] ClientAuditListQuery getAuditsQuery,
            CancellationToken cancellationToken) =>
        {
            var audits = await mediator.Send(getAuditsQuery, cancellationToken);
            return audits.ToHttpResult();
        });

        routeBuilder.MapPut("/clients/{clientId}/authorized-users/{userId}/archive", async (
            IMediator mediator,
            [AsParameters] ArchiveAuthorizedUserCommand command,
            CancellationToken cancellationToken = default) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return result.ToHttpResult();
        });

        routeBuilder.MapGet("/clients/{clientId}/authorized-users/count", async (
            IMediator mediator,
            [AsParameters] AuthorizedUsersCountQuery query,
            CancellationToken cancellationToken = default) =>
        {
            var authorizedUsersCount = await mediator.Send(query, cancellationToken);
            return authorizedUsersCount.ToHttpResult();
        });

        routeBuilder.MapGet("/clients/{clientId}/authorized-users", async (
            IMediator mediator,
            [AsParameters] AuthorizedUsersQuery query,
            CancellationToken cancellationToken = default) =>
        {
            var authorizedUsers = await mediator.Send(query, cancellationToken);
            return authorizedUsers.ToHttpResult();
        });

        routeBuilder.MapPost("/clients/{clientId}/authorized-users", async (
            IMediator mediator,
            HttpContext context,
            [FromRoute] ClientId clientId,
            [FromBody] CreateAuthorizedUserRequest request) =>
        {
            var result =
                await mediator.Send(new CreateAuthorizedUserCommand
                {
                    ClientId = clientId,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    UserInfo = context.GetUserInfo()
                }, context.RequestAborted);
            return result.ToHttpResult();
        });

        routeBuilder.MapPut("/clients/{clientId}/authorized-users/{userId}/suspend", async (
            IMediator mediator,
            HttpContext context,
            [FromRoute] ClientId clientId,
            [FromRoute] AuthorizedUserId userId) =>
        {
            var result = await mediator.Send(
                new ChangeAuthorizedUserStatusCommand
                {
                    ClientId = clientId,
                    UserId = userId,
                    Status = AuthorizedUserStatus.Suspended,
                    UserInfo = context.GetUserInfo()
                }, context.RequestAborted);

            return result.ToHttpResult();
        });

        routeBuilder.MapPut("/clients/{clientId}/authorized-users/{userId}/reactivate", async (
            IMediator mediator,
            HttpContext context,
            [FromRoute] ClientId clientId,
            [FromRoute] AuthorizedUserId userId) =>
        {
            var result = await mediator.Send(
                new ChangeAuthorizedUserStatusCommand
                {
                    ClientId = clientId,
                    UserId = userId,
                    Status = AuthorizedUserStatus.Active,
                    UserInfo = context.GetUserInfo()
                }, context.RequestAborted);

            return result.ToHttpResult();
        });

        routeBuilder.MapPut("/clients/{clientId}/bank-accounts/{bankAccountId}/archive", async (
            IMediator mediator,
            [AsParameters] ArchiveBankAccountCommand command,
            CancellationToken cancellationToken) =>
        {
            var archiveResult = await mediator.Send(command, cancellationToken);
            return archiveResult.ToHttpResult();
        });

        routeBuilder.MapGet("/clients/{clientId}/bank-accounts/{bankAccountId}", async (
            IMediator mediator,
            [AsParameters] BankAccountDetailsQuery query,
            CancellationToken cancellationToken = default) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return result.ToHttpResult();
        });

        routeBuilder.MapGet("/clients/{clientId}/bank-accounts", async (
            IMediator mediator,
            [AsParameters] BankAccountListQuery bankAccountsQuery,
            CancellationToken cancellationToken = default) =>
        {
            var bankAccounts = await mediator.Send(bankAccountsQuery, cancellationToken);
            return bankAccounts.ToHttpResult();
        });

        routeBuilder.MapGet("/clients/{clientId}/bank-accounts/{bankAccountId}/orders", async (
            IMediator mediator,
            [AsParameters] BankAccountOrderListQuery query,
            CancellationToken cancellationToken = default) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return result.ToHttpResult();
        });

        routeBuilder.MapPost("/clients/{clientId}/bank-accounts", async (
            IMediator mediator,
            [AsParameters] CreateBankAccountCommand command,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return result.ToHttpResult();
        });

        routeBuilder.MapPut("/clients/{clientId}/bank-accounts/{bankAccountId}/verify", async (
            IMediator mediator,
            [AsParameters] VerifyBankAccountCommand command,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return result.ToHttpResult();
        });

        routeBuilder.MapPut("/clients/{clientId}/bank-accounts/{bankAccountId}/decline", async (
            IMediator mediator,
            [AsParameters] DeclineBankAccountCommand command,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return result.ToHttpResult();
        });

        routeBuilder.MapPut("/clients/{clientId}/update-status", async (
            IMediator mediator,
            [FromRoute] ClientId clientId,
            [FromBody] ChangeStatusRequest changeStatusRequest,
            HttpContext context
        ) =>
        {
            var changeStatusResult = await mediator.Send(new ChangeClientStatusCommand
            {
                ClientId = clientId,
                Status = changeStatusRequest.Status,
                UserInfo = context.GetUserInfo(),
            }, context.RequestAborted);
            return changeStatusResult.ToHttpResult();
        });

        routeBuilder.MapGet("/clients/{clientId}/about", async (
            IMediator mediator,
            [AsParameters] ClientGeneralInfoQuery baseClientQuery,
            CancellationToken cancellationToken = default) =>
        {
            var baseClient = await mediator.Send(baseClientQuery, cancellationToken);
            return baseClient.ToHttpResult();
        });

        routeBuilder.MapGet("/clients", async (
            IMediator mediator,
            [AsParameters] ClientListQuery clientListQuery,
            CancellationToken cancellationToken
        ) =>
        {
            var clients = await mediator.Send(clientListQuery, cancellationToken);
            return clients.ToHttpResult();
        });

        routeBuilder.MapGet("/clients/{clientId}/documents/", async (
            IMediator mediator,
            [AsParameters] ComplianceDocumentsListQuery complianceDocumentsList,
            CancellationToken cancellationToken) =>
        {
            var listClientDocumentResult = await mediator.Send(complianceDocumentsList, cancellationToken);
            return listClientDocumentResult.ToHttpResult();
        });

        routeBuilder.MapPost("/clients/{clientId}/documents", async (
            IMediator mediator,
            [AsParameters] UploadComplianceDocumentsCommand uploadComplianceDocuments,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(uploadComplianceDocuments, cancellationToken);
            return result.ToHttpResult();
        }).DisableAntiforgery();

        routeBuilder.MapPut("/clients/{clientId}/fdt-accounts/{fdtAccountId}/archive", async (
            IMediator mediator,
            [AsParameters] ArchiveFdtAccountCommand archiveFdtAccountCommand,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await mediator.Send(archiveFdtAccountCommand, cancellationToken);
            return result.ToHttpResult();
        });

        routeBuilder.MapPost("/clients/{clientId}/fdt-accounts", async (
            IMediator mediator,
            [AsParameters] CreateFdtAccountCommand createFdtAccountCommand,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await mediator.Send(createFdtAccountCommand, cancellationToken);
            return result.ToHttpResult();
        });

        routeBuilder.MapGet("/clients/{clientId}/fdt-accounts/{fdtAccountId}", async (
            IMediator mediator,
            [AsParameters] FdtAccountDetailsQuery getFdtAccountDetails,
            CancellationToken cancellationToken) =>
        {
            var accountDetails = await mediator.Send(getFdtAccountDetails, cancellationToken);
            return accountDetails.ToHttpResult();
        });

        routeBuilder.MapGet("/clients/{clientId}/fdt-accounts", async (
            IMediator mediator,
            [AsParameters] FdtAccountListQuery getFdtAccounts,
            CancellationToken cancellationToken) =>
        {
            var accounts = await mediator.Send(getFdtAccounts, cancellationToken);
            return accounts.ToHttpResult();
        });

        routeBuilder.MapGet("/clients/{clientId}/fdt-accounts/{fdtAccountId}/orders", async (
            IMediator mediator,
            [AsParameters] FdtAccountOrderListQuery getFdtAccountOrders,
            CancellationToken cancellationToken) =>
        {
            var accountOrders = await mediator.Send(getFdtAccountOrders, cancellationToken);
            return accountOrders.ToHttpResult();
        });

        routeBuilder.MapPut("/clients/{clientId}/fdt-accounts/{fdtAccountId}/verify", async (
            IMediator mediator,
            [AsParameters] VerifyFdtAccountCommand command,
            CancellationToken cancellationToken
        ) =>
        {
            var verifyAccount = await mediator.Send(command, cancellationToken);
            return verifyAccount.ToHttpResult();
        });

        routeBuilder.MapPut("/clients/{clientId}/fdt-accounts/{fdtAccountId}/decline", async (
            IMediator mediator,
            [AsParameters] DeclineFdtAccountCommand command,
            CancellationToken cancellationToken
        ) =>
        {
            var verifyAccount = await mediator.Send(command, cancellationToken);
            return verifyAccount.ToHttpResult();
        });

        routeBuilder.MapGet("/clients/{clientId}/orders", async (
            IMediator mediator,
            [AsParameters] ClientOrderListQuery query,
            CancellationToken cancellationToken = default) =>
        {
            var clientOrders = await mediator.Send(query, cancellationToken);
            return clientOrders.ToHttpResult();
        });

        routeBuilder.MapGet("/clients/{clientId}/about/totals", async (
            IMediator mediator,
            [AsParameters] ClientOrderTotalsListQuery orderTotalsQuery,
            CancellationToken cancellationToken
        ) =>
        {
            var orderTotals = await mediator.Send(orderTotalsQuery, cancellationToken);
            return orderTotals.ToHttpResult();
        });

        routeBuilder.MapPut("/clients/{clientId}/deposit-banks/{depositBankId}", async (
            IMediator mediator,
            [AsParameters] ChangeDepositBankCommand changeDepositBankCommand,
            CancellationToken cancellationToken = default) =>
        {
            var changeDepositBankResult = await mediator.Send(changeDepositBankCommand, cancellationToken);
            return changeDepositBankResult.ToHttpResult();
        });

        routeBuilder.MapGet("/clients/{clientId}/overview/orders", async (
            IMediator mediator,
            [AsParameters] ClientOrderOverviewListQuery overviewOrdersQuery,
            CancellationToken cancellationToken = default) =>
        {
            var clientOverview = await mediator.Send(overviewOrdersQuery, cancellationToken);
            return clientOverview.ToHttpResult();
        });

        routeBuilder.MapGet("/clients/{clientId}/overview", async (
            IMediator mediator,
            [AsParameters] ClientOverviewQuery overviewQuery,
            CancellationToken cancellationToken = default) =>
        {
            var clientOverview = await mediator.Send(overviewQuery, cancellationToken);
            return clientOverview.ToHttpResult();
        });

        routeBuilder.MapGet("/clients/{clientId}/overview/deposit-banks", async (
            IMediator mediator,
            [AsParameters] OverviewDepositListQuery overviewDepositListQuery,
            CancellationToken cancellationToken = default) =>
        {
            var overviewDepositListResult = await mediator.Send(overviewDepositListQuery, cancellationToken);
            return overviewDepositListResult.ToHttpResult();
        });

        routeBuilder.MapPut("/clients/{clientId}/wallets/{walletId}/archive", async (
            IMediator mediator,
            [AsParameters] WalletArchivingCommand archiveWalletCommand,
            CancellationToken cancellationToken = default
        ) =>
        {
            var result = await mediator.Send(archiveWalletCommand, cancellationToken);
            return result.ToHttpResult();
        });

        routeBuilder.MapPost("/clients/{clientId}/wallets", async (
            IMediator mediator,
            [AsParameters] WalletCreationCommand createWalletCommand,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await mediator.Send(createWalletCommand, cancellationToken);
            return result.ToHttpResult();
        });

        routeBuilder.MapGet("/clients/{clientId}/wallets/{walletId}", async (
            IMediator mediator,
            [AsParameters] WalletDetailsQuery query,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return result.ToHttpResult();
        });

        routeBuilder.MapGet("/clients/{clientId}/wallets/{walletId}/orders", async (
            IMediator mediator,
            [AsParameters] WalletOrdersQuery query,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return result.ToHttpResult();
        });

        routeBuilder.MapGet("/clients/{clientId}/wallets", async (
            IMediator mediator,
            [AsParameters] WalletsQuery walletsQuery,
            CancellationToken cancellationToken
        ) =>
        {
            var wallets = await mediator.Send(walletsQuery, cancellationToken);
            return wallets.ToHttpResult();
        });

        routeBuilder.MapPut("/clients/{clientId}/wallets/{walletId}/verify", async (
            IMediator mediator,
            [AsParameters] VerifyWalletCommand command,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return result.ToHttpResult();
        });

        routeBuilder.MapPut("/clients/{clientId}/wallets/{walletId}/decline", async (
            IMediator mediator,
            [AsParameters] DeclineWalletCommand command,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return result.ToHttpResult();
        });
    }
}