using FluentValidation;
using SmartCoinOS.Domain.Services;
using SmartCoinOS.Integrations.AzureGraphApi.Contracts;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.AuthorizedUsers;

public sealed class SetAuthorizedUsersFormCommandValidator : AbstractValidator<SetAuthorizedUsersFormCommand>
{
    public SetAuthorizedUsersFormCommandValidator(IGraphApiClient graphApiClient)
    {
        RuleFor(x => x.AuthorizedUsers).NotEmpty();

        var documentTypeService = DocumentTypeService.Instance;
        RuleForEach(x => x.AuthorizedUsers)
            .ChildRules(c =>
            {
                c.RuleFor(x => x.FirstName)
                    .NotEmpty().WithMessage("First name is required")
                    .Length(2, 50).WithMessage("First name must be between 2 and 50 characters")
                    .Matches(@"^[a-zA-Z]+$").WithMessage("First name must be alphabetical");

                c.RuleFor(x => x.LastName)
                    .NotEmpty().WithMessage("Last name is required")
                    .Length(2, 50).WithMessage("Last name must be between 2 and 50 characters")
                    .Matches(@"^[a-zA-Z]+$").WithMessage("Last name must be alphabetical");

                c.RuleFor(x => x.UserId)
                    .NotEmpty().WithMessage("User ID is required")
                    .Length(3, 50).WithMessage("User ID must be between 3 and 50 characters")
                    .EmailAddress().WithMessage("User ID must be a valid email address");

                c.RuleFor(x => x.PersonalVerification)
                    .Must(x => documentTypeService.IsIdentityProof(x!.DocumentType))
                    .When(x => x.PersonalVerification is not null);

                c.RuleFor(x => x.ProofOfAddress)
                    .Must(x => documentTypeService.IsAddressProof(x!.DocumentType))
                    .When(x => x.ProofOfAddress is not null);

                c.RuleFor(x => x.UserId)
                    .MustAsync(async (user, email, cancellationToken) =>
                    {
                        var emailAlreadyExists = await graphApiClient.ExistsAsync(email, cancellationToken);
                        return !emailAlreadyExists;
                    }).WithMessage("User {PropertyValue} already exists. Please use another id.");
            });
    }
}