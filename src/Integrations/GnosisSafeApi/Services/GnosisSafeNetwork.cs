namespace SmartCoinOS.Integrations.GnosisSafeApi.Services;

/// <summary>
/// We only support the networks enumerated below. 
/// See https://docs.safe.global/safe-core-api/supported-networks for all networks supported by Safe{Core} API.
/// </summary>
public enum GnosisSafeNetwork
{
    /// <summary>
    /// Sepolia Test network https://safe-transaction-sepolia.safe.global/ 
    /// </summary>
    Sepolia,

    /// <summary>
    /// Etherium main net https://safe-transaction-mainnet.safe.global/
    /// </summary>
    Etherium,

    /// <summary>
    /// BNB Smart Chain https://safe-transaction-bsc.safe.global/
    /// </summary>
    Bnb
}
