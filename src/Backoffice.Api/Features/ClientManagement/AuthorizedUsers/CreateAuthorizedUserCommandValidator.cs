using FluentValidation;
using SmartCoinOS.Integrations.AzureGraphApi.Contracts;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.AuthorizedUsers;

public sealed class CreateAuthorizedUserCommandValidator : AbstractValidator<CreateAuthorizedUserCommand>
{
    public CreateAuthorizedUserCommandValidator(IGraphApiClient graphApiClient)
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .Length(2, 50);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .Length(2, 50);

        RuleFor(x => x.Email)
            .NotEmpty()
            .Length(3, 50)
            .EmailAddress();

        RuleFor(x => x.Email)
            .MustAsync(async (email, cancellation) =>
            {
                var user = await graphApiClient.FindUserByEmailAsync(email, cancellation);
                return user is null;
            }).WithMessage("User with provided email already exists");
    }
}