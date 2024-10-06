using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Domain.Base;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Minting;

public sealed class MintOrderCommandValidator : AbstractValidator<MintOrderCommand>
{
    public MintOrderCommandValidator(ReadOnlyDataContext context)
    {
        RuleFor(x => x.Request.ClientId).NotEmpty();
        RuleFor(x => x.Request.MintAmount)
            .NotEmpty()
            .Must(x => x.Amount > 0).WithMessage("Mint amount should be more then 0.");

        RuleFor(x => x.Request.MintAmount.AssetSymbol)
            .Must(x => GlobalConstants.Currency.SupportedCurrencies().Any(c => c.Equals(x)))
            .WithMessage($"The supplied currency is unknown.");

        RuleFor(x => x.Request.WalletId).NotEmpty();

        RuleFor(x => x.Request)
            .Must(request => (request.BankAccountId.HasValue && !request.FdtAccountId.HasValue) ||
                             (!request.BankAccountId.HasValue && request.FdtAccountId.HasValue))
            .WithMessage("One of BankAccount or FdtAccount should be specified");

        RuleFor(x => x.Request.BankAccountId)
            .MustAsync(async (command, bankAccountId, cancellationToken) =>
            {
                if (!bankAccountId.HasValue)
                    return true;

                var clientOwnsBank = await context.BankAccounts
                    .AnyAsync(b => b.Id == bankAccountId
                                   && b.VerificationStatus == EntityVerificationStatus.Verified
                                   && b.ClientId == command.Request.ClientId, cancellationToken: cancellationToken);

                return clientOwnsBank;
            }).WithMessage("Couldn't find this bank account");

        RuleFor(x => x.Request.FdtAccountId)
            .MustAsync(async (command, fdtAccountId, cancellationToken) =>
            {
                if (!fdtAccountId.HasValue)
                    return true;

                var clientOwnsFdsAcc = await context.FdtAccounts
                    .AnyAsync(b => b.Id == fdtAccountId
                                   && b.VerificationStatus == EntityVerificationStatus.Verified
                                   && b.ClientId == command.Request.ClientId, cancellationToken: cancellationToken);

                return clientOwnsFdsAcc;
            }).WithMessage("Couldn't find this fdt account");

        RuleFor(x => x.Request.WalletId)
            .MustAsync(async (command, walletId, cancellationToken) =>
            {
                var clientOwnsWallet = await context.Wallets
                    .AnyAsync(b => b.Id == walletId
                                   && b.VerificationStatus == EntityVerificationStatus.Verified
                                   && b.ClientId == command.Request.ClientId, cancellationToken: cancellationToken);

                return clientOwnsWallet;
            }).WithMessage("Couldn't find this wallet");

        RuleFor(x => x.Request.ClientId)
            .MustAsync(async (_, clientId, cancellationToken) =>
            {
                var clientStatus = await context.Clients.Where(x => x.Id == clientId)
                    .Select(x => x.Status)
                    .FirstAsync(cancellationToken);

                return clientStatus == ClientStatus.Active;
            }).WithMessage("Client status doesn't permit creating orders");
    }
}
