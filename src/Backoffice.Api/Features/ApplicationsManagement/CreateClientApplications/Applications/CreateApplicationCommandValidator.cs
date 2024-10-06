using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace SmartCoinOS.Backoffice.Api.Features.ApplicationsManagement.CreateClientApplications.Applications;

public sealed class CreateApplicationCommandValidator : AbstractValidator<CreateApplicationCommand>
{
    public CreateApplicationCommandValidator(ReadOnlyDataContext context)
    {
        RuleFor(x => x.Application.LegalEntityName)
            .NotEmpty()
            .Length(3, 30)
            .MustAsync(async (legalName, cancellationToken) =>
            {
                legalName = legalName.Trim();

                var clientExists = await context.Applications.AnyAsync(x => x.LegalEntityName == legalName, cancellationToken);
                return !clientExists;
            }).WithMessage("Client with the provided Legal Entity Name already exists.");
    }
}
