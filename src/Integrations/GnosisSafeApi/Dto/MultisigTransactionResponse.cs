namespace SmartCoinOS.Integrations.GnosisSafeApi.Dto;

public sealed record MultisigTransactionResponse
{
    public required string Safe { get; set; }
    public required string To { get; set; }
    public required string Value { get; set; }
    public string? Data { get; set; }
    public int Operation { get; set; }
    public string? GasToken { get; set; }
    public long SafeTxGas { get; set; }
    public long BaseGas { get; set; }
    public string? GasPrice { get; set; }
    public string? RefundReceiver { get; set; }
    public long Nonce { get; set; }
    public DateTimeOffset? ExecutionDate { get; set; }
    public DateTimeOffset? SubmissionDate { get; set; }
    public DateTimeOffset? Modified { get; set; }
    public long? BlockNumber { get; set; }
    public string? TransactionHash { get; set; }
    public required string SafeTxHash { get; set; }
    public required string Proposer { get; set; }
    public string? Executor { get; set; }
    public bool IsExecuted { get; set; }
    public bool? IsSuccessful { get; set; }
    public string? EthGasPrice { get; set; }
    public string? MaxFeePerGas { get; set; }
    public string? MaxPriorityFeePerGas { get; set; }
    public long? GasUsed { get; set; }
    public long? Fee { get; set; }
    public string? Origin { get; set; }
    public string? DataDecoded { get; set; }
    public long ConfirmationsRequired { get; set; }
    public List<SafeMultisigConfirmationResponse> Confirmations { get; set; } = [];
    public bool? Trusted { get; set; }
    public string? Signatures { get; set; }
}