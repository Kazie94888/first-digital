namespace SmartCoinOS.Integrations.GnosisSafeApi.Dto;
public sealed record SafeMultisigConfirmationResponse
{
    public required string Owner { get; set; }
    public required DateTimeOffset SubmissionDate { get; set; }
    public string? TransactionHash { get; set; }
    public required string Signature { get; set; }
    public string? SignatureType { get; set; }
}
