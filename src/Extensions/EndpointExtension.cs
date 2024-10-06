using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SmartCoinOS.Extensions;

public static class EndpointExtension
{
    public static void AddEndpoints(this WebApplicationBuilder builder)
    {
        var endpointServiceDescriptors = Assembly.GetCallingAssembly().DefinedTypes
            .Where(type =>
                type is { IsAbstract: false, IsInterface: false }
                && type.IsAssignableTo(typeof(IEndpointBuilder)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpointBuilder), type))
            .ToArray();

        builder.Services.TryAddEnumerable(endpointServiceDescriptors);
    }

    public static void MapEndpoints(this WebApplication application)
    {
        var endpoints = application.Services.GetRequiredService<IEnumerable<IEndpointBuilder>>();
        foreach (var endpoint in endpoints)
        {
            endpoint.Map(application);
        }
    }
}