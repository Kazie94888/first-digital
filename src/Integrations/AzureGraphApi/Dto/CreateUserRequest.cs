namespace SmartCoinOS.Integrations.AzureGraphApi.Dto;

public sealed record CreateUserRequest(string FirstName, string LastName, string Email, string Password);