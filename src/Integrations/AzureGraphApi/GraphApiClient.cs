using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using SmartCoinOS.Integrations.AzureGraphApi.Contracts;
using SmartCoinOS.Integrations.AzureGraphApi.Dto;
using SmartCoinOS.Integrations.AzureGraphApi.Options;

namespace SmartCoinOS.Integrations.AzureGraphApi;

public sealed class GraphApiClient : IGraphApiClient
{
    private const string _creationTypeFilter = "creationType eq 'LocalAccount'";

    private static readonly string[] _selectProperties =
    [
        "id",
        "givenName",
        "displayName",
        "surname",
        "mail"
    ];

    private readonly GraphServiceClient _client;
    private readonly GraphApiConfig _config;

    public GraphApiClient(GraphServiceClient client, IOptions<GraphApiConfig> config)
    {
        _client = client;
        _config = config.Value;
    }

    public async Task<UserDto[]> GetAllUsersAsync(CancellationToken cancellationToken)
    {
        var users = new List<User>();
        var response = await _client.Users.GetAsync(request =>
        {
            request.QueryParameters.Select = _selectProperties;
            request.QueryParameters.Filter = _creationTypeFilter;
        }, cancellationToken);

        if (response == null)
            return [];

        var pageIterator = PageIterator<User, UserCollectionResponse>
            .CreatePageIterator(_client, response, (user) =>
            {
                users.Add(user);
                return true;
            });

        await pageIterator.IterateAsync(cancellationToken);

        return users
            .Select(ToUserDto)
            .ToArray();
    }

    public async Task<UserDto?> FindUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var response = await _client.Users.GetAsync(request =>
        {
            request.QueryParameters.Select = _selectProperties;
            request.QueryParameters.Filter = $"{_creationTypeFilter} and mail eq '{email}'";
        }, cancellationToken);

        var user = response?.Value?.FirstOrDefault();
        return user is null ? null : ToUserDto(user);
    }

    public async Task<bool> ExistsAsync(string email, CancellationToken cancellationToken)
    {
        var response = await _client.Users.GetAsync(request =>
        {
            request.QueryParameters.Filter = $"{_creationTypeFilter} and mail eq '{email}'";
        }, cancellationToken);

        var user = response?.Value?.FirstOrDefault();
        return user is not null;
    }

    public async Task<string> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _client.Users.PostAsync(new User
        {
            AccountEnabled = true,
            DisplayName = $"{request.FirstName} {request.LastName}",
            GivenName = request.FirstName,
            Surname = request.LastName,
            Mail = request.Email,
            PasswordProfile = new PasswordProfile
            {
                Password = request.Password,
                ForceChangePasswordNextSignIn = true
            },
            PasswordPolicies = "DisablePasswordExpiration",
            CreationType = "LocalAccount",
            Identities =
            [
                new()
                {
                    SignInType = "emailAddress",
                    Issuer = _config.TenantDomainName,
                    IssuerAssignedId = request.Email
                }
            ]
        }, cancellationToken: cancellationToken);

        return user!.Id!;
    }

    public Task DeleteUserAsync(string id, CancellationToken cancellationToken)
    {
        return _client.Users[id].DeleteAsync(cancellationToken: cancellationToken);
    }

    private static UserDto ToUserDto(User azureUser)
    {
        ArgumentNullException.ThrowIfNull(azureUser);
        ArgumentNullException.ThrowIfNull(azureUser.Id);

        if (string.IsNullOrEmpty(azureUser.Mail)
            || string.IsNullOrEmpty(azureUser.GivenName)
            || string.IsNullOrEmpty(azureUser.Surname)
            || string.IsNullOrEmpty(azureUser.DisplayName))
            throw new ArgumentNullException(
                $"User '{azureUser.Id}' is missing one or more of the required properties (Name, Surname, Display Name or Email).");

        return new UserDto(azureUser.Id, azureUser.DisplayName, azureUser.GivenName, azureUser.Surname, azureUser.Mail);
    }
}
