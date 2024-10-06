namespace SmartCoinOS.Integrations.CourierApi.DependencyInjection;

public sealed record CourierSettings
{
    public required string FirstTimeLoginTemplate { get; init; }
    public required string ApiKey { get; init; }
    public required string ApiUrl { get; init; }
}
