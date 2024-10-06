namespace SmartCoinOS.Integrations.GnosisSafeApi.Dto;

public sealed record SafeResponse
{
    public required string Address { get; set; }
    public int Nonce { get; set; }
    public int Threshold { get; set; }
    public List<string> Owners { get; set; } = [];
    public required string MasterCopy { get; set; }
    public List<string> Modules { get; set; } = [];
    public required string FallbackHandler { get; set; }
    public required string Guard { get; set; }
    public required string Version { get; set; }
}
