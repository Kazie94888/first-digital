namespace SmartCoinOS.Integrations.GnosisSafeApi.Dto;

public sealed record ConfirmationsListResponse
{
    public required int Count { get; set; }
    public string? Next { get; set; }
    public string? Previous { get; set; }
    public required List<SafeMultisigConfirmationResponse> Results { get; set; }
}
