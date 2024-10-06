namespace SmartCoinOS.Integrations.GnosisSafeApi.Dto;

public sealed record FindMySafesResponse
{
    public required List<string> Safes { get; set; } = [];
}
