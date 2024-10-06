using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Extensions;

namespace SmartCoinOS.ClientPortal.Api.Features.ValidationSchema;

internal class EndpointBuilder : IEndpointBuilder
{
    public void Map(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("/validations/{commandName}", (
                HttpContext context,
                ValidationSchemaProvider resolver,
                [FromRoute] string commandName
            ) =>
            {
                var schema = resolver.GetValidationSchema(commandName);
                context.Response.Headers.ETag = $"\"{schema.Hash}\"";
                return Results.Text(schema.JsonSchema, MediaTypeNames.Application.Json);
            })
            .CacheOutput();
    }
}