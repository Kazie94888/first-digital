using MediatR;
using SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.Applications;
using SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.AuthorizedUsers;
using SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.BankAccounts;
using SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.BusinessInfo;
using SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.Documents;
using SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.EntityParticulars;
using SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.FdtAccounts;
using SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.Metadata;
using SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.Wallets;
using SmartCoinOS.Extensions;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications;

internal class EndpointBuilder : IEndpointBuilder
{
    public void Map(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost("/applications", async (
            IMediator mediator,
            [AsParameters] CreateApplicationCommand createApplicationCommand,
            CancellationToken cancellationToken
        ) =>
        {
            var createApplicationResult = await mediator.Send(createApplicationCommand, cancellationToken);
            return createApplicationResult.ToHttpResult();
        });

        routeBuilder.MapGet("/applications", async (
            IMediator mediator,
            [AsParameters] ApplicationListQuery applicationListQuery,
            CancellationToken cancellationToken
        ) =>
        {
            var applicationListResult = await mediator.Send(applicationListQuery, cancellationToken);
            return applicationListResult.ToHttpResult();
        });

        routeBuilder.MapPut("/applications/{applicationId}/submit", async (
            IMediator mediator,
            [AsParameters] SubmitApplicationCommand submitApplicationCommand,
            CancellationToken cancellationToken) =>
        {
            var submitApplicationResult = await mediator.Send(submitApplicationCommand, cancellationToken);
            return submitApplicationResult.ToHttpResult();
        });

        routeBuilder.MapGet("/applications/{applicationId}/authorized-users", async (
            IMediator mediator,
            [AsParameters] AuthorizedUsersFormQuery authorizedUsersFormQuery,
            CancellationToken cancellationToken) =>
        {
            var authorizedUsersFormResult = await mediator.Send(authorizedUsersFormQuery, cancellationToken);
            return authorizedUsersFormResult.ToHttpResult();
        });

        routeBuilder.MapPost("/applications/{applicationId}/authorized-users", async (
            IMediator mediator,
            [AsParameters] SetAuthorizedUsersFormCommand setAuthorizedUsersCommand,
            CancellationToken cancellationToken) =>
        {
            var setAuthorizedUsersResult = await mediator.Send(setAuthorizedUsersCommand, cancellationToken);
            return setAuthorizedUsersResult.ToHttpResult();
        });

        routeBuilder.MapGet("/applications/{applicationId}/bank-accounts", async (
            IMediator mediator,
            [AsParameters] BankAccountsFormQuery bankAccountsFormQuery,
            CancellationToken cancellationToken) =>
        {
            var bankAccountsFormResult = await mediator.Send(bankAccountsFormQuery, cancellationToken);
            return bankAccountsFormResult.ToHttpResult();
        });

        routeBuilder.MapPost("/applications/{applicationId}/bank-accounts", async (
            IMediator mediator,
            [AsParameters] SetBankAccountsFormCommand setBankAccountsFormCommand,
            CancellationToken cancellationToken
        ) =>
        {
            var setBankAccountsFormResult = await mediator.Send(setBankAccountsFormCommand, cancellationToken);
            return setBankAccountsFormResult.ToHttpResult();
        });

        routeBuilder.MapGet("/applications/{applicationId}/business-info", async (
            IMediator mediator,
            [AsParameters] BusinessInfoFormQuery businessInfoFormQuery,
            CancellationToken cancellationToken) =>
        {
            var businessInfoFormResult = await mediator.Send(businessInfoFormQuery, cancellationToken);
            return businessInfoFormResult.ToHttpResult();
        });

        routeBuilder.MapPost("/applications/{applicationId}/business-info", async (
            IMediator mediator,
            [AsParameters] SetBusinessInfoFormCommand setBusinessInfoFormCommand,
            CancellationToken cancellationToken
        ) =>
        {
            var setBusinessInfoFormResult = await mediator.Send(setBusinessInfoFormCommand, cancellationToken);
            return setBusinessInfoFormResult.ToHttpResult();
        });

        routeBuilder.MapGet("/applications/{applicationId}/documents", async (
            IMediator mediator,
            [AsParameters] DocumentsFormQuery documentsFormQuery,
            CancellationToken cancellationToken
        ) =>
        {
            var documentsFormResult = await mediator.Send(documentsFormQuery, cancellationToken);
            return documentsFormResult.ToHttpResult();
        });

        routeBuilder.MapPost("/applications/{applicationId}/documents", async (
            IMediator mediator,
            [AsParameters] SetDocumentsFormCommand setDocumentsFormCommand,
            CancellationToken cancellationToken
        ) =>
        {
            var setDocumentsFormResult = await mediator.Send(setDocumentsFormCommand, cancellationToken);
            return setDocumentsFormResult.ToHttpResult();
        });
        routeBuilder.MapGet("/applications/{applicationId}/entity-particulars", async (
            IMediator mediator,
            [AsParameters] GetEntityParticularsFormQuery getEntityParticularsFormQuery,
            CancellationToken cancellationToken) =>
        {
            var getEntityParticularsFormResult = await mediator.Send(getEntityParticularsFormQuery, cancellationToken);
            return getEntityParticularsFormResult.ToHttpResult();
        });

        routeBuilder.MapPost("/applications/{applicationId}/entity-particulars", async (
            IMediator mediator,
            [AsParameters] SetEntityParticularsFormCommand setEntityParticularsFormCommand,
            CancellationToken cancellationToken
        ) =>
        {
            var setEntityParticularsFormResult = await mediator.Send(setEntityParticularsFormCommand, cancellationToken);
            return setEntityParticularsFormResult.ToHttpResult();
        });

        routeBuilder.MapPost("/applications/{applicationId}/fdt-accounts", async (
            IMediator mediator,
            [AsParameters] SetFdtAccountsFormCommand setFdtAccountsFormCommand,
            CancellationToken cancellationToken
        ) =>
        {
            var setFdtAccountsFormResult = await mediator.Send(setFdtAccountsFormCommand, cancellationToken);
            return setFdtAccountsFormResult.ToHttpResult();
        });

        routeBuilder.MapGet("/applications/{applicationId}/fdt-accounts", async (
            IMediator mediator,
            [AsParameters] FdtAccountsFormQuery fdtAccountsFormQuery,
            CancellationToken cancellationToken) =>
        {
            var fdtAccountsFormResult = await mediator.Send(fdtAccountsFormQuery, cancellationToken);
            return fdtAccountsFormResult.ToHttpResult();
        });

        routeBuilder.MapGet("/applications/{applicationId}/metadata", async (
            IMediator mediator,
            [AsParameters] MetadataQuery metadataQuery,
            CancellationToken cancellationToken) =>
        {
            var metadataQueryResult = await mediator.Send(metadataQuery, cancellationToken);
            return metadataQueryResult.ToHttpResult();
        });

        routeBuilder.MapPost("/applications/{applicationId}/wallets", async (
            IMediator mediator,
            [AsParameters] SetWalletsFormCommand setWalletsFormCommand,
            CancellationToken cancellationToken
        ) =>
        {
            var setWalletsFormResult = await mediator.Send(setWalletsFormCommand, cancellationToken);
            return setWalletsFormResult.ToHttpResult();
        });

        routeBuilder.MapGet("/applications/{applicationId}/wallets", async (
            IMediator mediator,
            [AsParameters] WalletsFormQuery walletsFormQuery,
            CancellationToken cancellationToken) =>
        {
            var walletsFormResult = await mediator.Send(walletsFormQuery, cancellationToken);
            return walletsFormResult.ToHttpResult();
        });
    }
}