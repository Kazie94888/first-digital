using MediatR;
using SmartCoinOS.Backoffice.Api.Base;
using SmartCoinOS.Backoffice.Api.Base.WebHooks;

namespace SmartCoinOS.Backoffice.Api.Features.Webhooks.SmartTrust;

public sealed class SmartTrustWebhook : IWebHook
{
    public static string AuthorizationPolicy => ApplicationConstants.PolicyNames.SmartTrustAuthPolicy;
    private readonly IMediator _mediator;
    public SmartTrustWebhook(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IResult> HandleAsync(HttpContext context, CancellationToken cancellationToken)
    {
        using var streamReader = new StreamReader(context.Request.Body);
        var requestBody = await streamReader.ReadToEndAsync(cancellationToken);

        var stCommand = SmartTrustFactory.GetCommand(requestBody);

        if (stCommand.IsFailed)
            return stCommand.ToHttpResult();

        var commandResult = await _mediator.Send(stCommand.Value, cancellationToken);
        return commandResult.ToHttpResult();
    }
}
