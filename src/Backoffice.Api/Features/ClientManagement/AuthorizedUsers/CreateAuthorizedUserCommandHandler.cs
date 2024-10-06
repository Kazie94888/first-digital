using FluentResults;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Infrastructure.Helpers.RandomStringGeneration;
using SmartCoinOS.Integrations.AzureGraphApi.Contracts;
using SmartCoinOS.Integrations.AzureGraphApi.Dto;
using SmartCoinOS.Integrations.CourierApi.Contracts;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.AuthorizedUsers;

internal sealed class CreateAuthorizedUserCommandHandler : ICommandHandler<CreateAuthorizedUserCommand, EntityId>
{
    private readonly DataContext _context;
    private readonly IGraphApiClient _graphApiClient;
    private readonly ICourierApiClient _courierApiClient;

    public CreateAuthorizedUserCommandHandler(DataContext context, IGraphApiClient graphApiClient,
        ICourierApiClient courierApiClient)
    {
        _context = context;
        _graphApiClient = graphApiClient;
        _courierApiClient = courierApiClient;
    }

    public async Task<Result<EntityId>> Handle(CreateAuthorizedUserCommand command, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FirstAsync(x => x.Id == command.ClientId, cancellationToken);

        var password = RandomGenerator.GenerateString();

        var userId = await _graphApiClient.CreateUserAsync(
            new CreateUserRequest(
                command.FirstName,
                command.LastName,
                command.Email,
                password),
            cancellationToken);

        var addResult = client.AddAuthorizedUser(command.FirstName, command.LastName, command.Email, userId, command.UserInfo);

        if (addResult.IsFailed)
            return Result.Fail(addResult.Errors);

        await _courierApiClient.SendCredentialsEmailAsync($"{command.FirstName} {command.LastName}", command.Email, password, cancellationToken);

        var authorizedUser = addResult.Value;

        return Result.Ok(new EntityId(authorizedUser.Id.Value));
    }
}