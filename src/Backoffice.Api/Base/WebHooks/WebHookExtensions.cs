using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SmartCoinOS.Backoffice.Api.Base.WebHooks;

public static class WebHookExtensions
{
    public static void AddWebHook<TWebHook>(this IServiceCollection services)
        where TWebHook : class, IWebHook
    {
        services.TryAddTransient<TWebHook>();
    }

    public static RouteHandlerBuilder MapWebHook<TWebHook>(this IEndpointRouteBuilder builder, string pattern)
        where TWebHook : class, IWebHook
    {
        return builder.MapPost(pattern, async (HttpContext context, TWebHook webHook) => await webHook.HandleAsync(context, context.RequestAborted))
            .RequireAuthorization(TWebHook.AuthorizationPolicy);
    }
}