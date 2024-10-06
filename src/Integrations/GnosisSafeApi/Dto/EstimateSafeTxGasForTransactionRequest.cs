namespace SmartCoinOS.Integrations.GnosisSafeApi.Dto;

public sealed record EstimateSafeTxGasForTransactionRequest
{
    public required string To { get; set; }
    public required int Value { get; set; }
    public string? Data { get; set; }
    public required int Operation { get; set; }
}
