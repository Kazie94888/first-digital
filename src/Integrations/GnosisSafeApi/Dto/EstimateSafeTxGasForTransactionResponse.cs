namespace SmartCoinOS.Integrations.GnosisSafeApi.Dto;

public sealed record EstimateSafeTxGasForTransactionResponse
{
    public required string SafeTxGas { get; set; }
}