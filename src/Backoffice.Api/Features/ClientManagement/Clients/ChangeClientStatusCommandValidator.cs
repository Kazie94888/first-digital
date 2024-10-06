using FluentValidation;
using SmartCoinOS.Domain.Clients;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Clients;

public sealed class ChangeClientStatusCommandValidator : AbstractValidator<ChangeClientStatusCommand>
{
    public ChangeClientStatusCommandValidator()
    {
        RuleFor(x => x.Status)
                .Must(v =>
                {
                    var expectedStatuses = new[] { ClientStatus.Active, ClientStatus.Inactive, ClientStatus.Dormant, ClientStatus.TemporarlySuspended };
                    return expectedStatuses.Contains(v);
                }).WithMessage("Status was not expected");
    }
}