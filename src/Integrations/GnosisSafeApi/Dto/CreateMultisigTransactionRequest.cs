namespace SmartCoinOS.Integrations.GnosisSafeApi.Dto;
public sealed record CreateMultisigTransactionRequest
{
    public required string Safe { get; set; }
    public required string To { get; set; }
    public required long Value { get; set; }
    public string? Data { get; set; }
    public required int Operation { get; set; }
    public string? GasToken { get; set; }
    public required int SafeTxGas { get; set; }
    public required int BaseGas { get; set; }
    public required int GasPrice { get; set; }
    public string? RefundReceiver { get; set; }
    public required int Nonce { get; set; }
    /// <summary>
    /// Still can't figure out where this is coming from.
    /// 
    /// See origin source tests: https://github.com/safe-global/safe-transaction-service/blob/252d656f65989c18372752298f4fbbda60ab7fce/safe_transaction_service/history/tests/test_views.py#L1299
    /// </summary>
    public required string ContractTransactionHash { get; set; }

    /// <summary>
    /// One of the safe owners
    /// </summary>
    public required string Sender { get; set; }
    public string? Signature { get; set; }
    public string? Origin { get; set; }
}
