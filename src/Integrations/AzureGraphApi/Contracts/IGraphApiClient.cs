using SmartCoinOS.Integrations.AzureGraphApi.Dto;

namespace SmartCoinOS.Integrations.AzureGraphApi.Contracts;

public interface IGraphApiClient
{
    Task<UserDto[]> GetAllUsersAsync(CancellationToken cancellationToken);
    Task<UserDto?> FindUserByEmailAsync(string email, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(string email, CancellationToken cancellationToken);
    Task<string> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken);
    Task DeleteUserAsync(string id, CancellationToken cancellationToken);
}