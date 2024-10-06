namespace SmartCoinOS.Integrations.AzureGraphApi.Dto;

public sealed record UserDto(string Id, string DisplayName, string FirstName, string LastName, string Email);