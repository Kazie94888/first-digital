using FluentValidation;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Wallets;

public sealed class WalletCreationValidator : AbstractValidator<WalletCreationCommand>
{
    public WalletCreationValidator()
    {
        RuleFor(x => x.Wallet.Address)
                .NotEmpty()
                .Length(10, 100);

        RuleFor(x => x.Wallet.Network).NotEmpty();

        RuleFor(x => x.Wallet.Alias)
            .MaximumLength(250);
    }
}

