using SmartCoinOS.Integrations.GnosisSafeApi.Dto;
using SmartCoinOS.Integrations.GnosisSafeApi.Services;

namespace SmartCoinOS.Integrations.GnosisSafeApi.Contracts;
public interface IGnosisSafeApiClient
{
    /// <summary>
    /// Returns all safes owned by this wallet address.
    /// Useful in cases when the client doesn't know how to find his safe identifiers.
    /// 
    /// The output of this can then be used to get safe details <see cref="GetSafe(GnosisSafeNetwork, string, CancellationToken)"/>
    /// </summary>
    /// <param name="network">Network to query, see <see cref="GnosisSafeNetwork"/></param>
    /// <param name="ownerAddress">
    /// This is the wallet identifier. Get this from provider (i.e., from MetaMask)
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<FindMySafesResponse> FindOwnerSafes(GnosisSafeNetwork network, string ownerAddress, CancellationToken cancellationToken);

    /// <summary>
    /// Return the **Safe Details** by address. This is useful to get the owners, see which owner didn't sign a transaction, or get the current nonce that is needed to reference when creating other transactions.
    /// </summary>
    /// <param name="network">Network to query, see <see cref="GnosisSafeNetwork"/></param>
    /// <param name="safeAddress">
    /// This is the safe unique identifier
    /// 
    /// i.e., 0x421558f7f5590b23442c41CE1e5382Bf901a890C
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<SafeResponse> GetSafe(GnosisSafeNetwork network, string safeAddress, CancellationToken cancellationToken);

    /// <summary>
    /// Get the details of a particular transaction.
    /// Useful i.e., to see the amount of signatures required for this transaction.
    /// </summary>
    /// <param name="network">Network to query, see <see cref="GnosisSafeNetwork"/></param>
    /// <param name="safeTxHash">This is the transaction unique identifier, it is created when the transaction itself is created</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<MultisigTransactionResponse> GetMultisigTransaction(GnosisSafeNetwork network, string safeTxHash, CancellationToken cancellationToken);

    /// <summary>
    /// Get the list of **confirmations** for a multisig transaction. See <see cref="SafeMultisigConfirmationResponse"/>
    /// /v1/multisig-transactions/{safe_tx_hash}/confirmations/
    /// </summary>
    /// <param name="network">Network to query, see <see cref="GnosisSafeNetwork"/></param>
    /// <param name="safeTxHash">This is the transaction unique identifier, it is created when the transaction itself is created</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ConfirmationsListResponse> GetMultisignTransactionConfirmationsAsync(GnosisSafeNetwork network, string safeTxHash, CancellationToken cancellationToken);


    /// <summary>
    /// Creates a Multisig Transaction with its confirmations and retrieves all the information related.
    /// /v1/safes/{address}/multisig-transactions/
    /// </summary>
    /// <param name="network"></param>
    /// <param name="address"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task CreateMultisigTransactionAsync(GnosisSafeNetwork network, string address, CreateMultisigTransactionRequest request, CancellationToken cancellationToken);


    /// <summary>
    /// Estimates `safeTxGas` for a Safe Multisig Transaction.
    /// /v1/safes/{address}/multisig-transactions/estimations/
    /// </summary>
    /// <param name="network"></param>
    /// <param name="address"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<EstimateSafeTxGasForTransactionResponse> EstimateSafeTxGasAsync(GnosisSafeNetwork network, string address, EstimateSafeTxGasForTransactionRequest request, CancellationToken cancellationToken);
}
