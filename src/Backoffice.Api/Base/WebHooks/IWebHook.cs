namespace SmartCoinOS.Backoffice.Api.Base.WebHooks;

public interface IWebHook
{
    Task<IResult> HandleAsync(HttpContext context, CancellationToken cancellationToken);

    static abstract string AuthorizationPolicy { get; }
}