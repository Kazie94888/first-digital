using FluentResults;
using Microsoft.Extensions.Options;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Options;

namespace SmartCoinOS.Backoffice.Api.Features.Settings.Blockchain;

internal sealed class BlockchainSettingsQueryHandler : IQueryHandler<BlockchainSettingsQuery, BlockchainSettingsResponse>
{
    private readonly BlockchainSettings _settings;
    public BlockchainSettingsQueryHandler(IOptionsSnapshot<BlockchainSettings> blockchainSettings)
    {
        _settings = blockchainSettings.Value;
    }
    public Task<Result<BlockchainSettingsResponse>> Handle(BlockchainSettingsQuery request, CancellationToken cancellationToken)
    {
        var settingsResult = new BlockchainSettingsResponse
        {
            SafeAddress = _settings.SafeAddress,
            SmartCoinAddress = _settings.SmartCoinAddress,
            Signers = _settings.Signers,
        };

        return Task.FromResult(Result.Ok(settingsResult));
    }
}