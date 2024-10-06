using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using AWSServiceProvider;
using AWSServiceProvider.Services.S3;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using NSwag;
using SmartCoinOS.ClientPortal.Api.Application.Behaviors;
using SmartCoinOS.ClientPortal.Api.Base;
using SmartCoinOS.ClientPortal.Api.Features.ValidationSchema;
using SmartCoinOS.ClientPortal.Api.Infrastructure;
using SmartCoinOS.Commands;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Integrations.AzureGraphApi.DependencyInjection;
using SmartCoinOS.Integrations.AzureGraphApi.Options;
using SmartCoinOS.Integrations.FdtPartnerApi.DependencyInjection;
using SmartCoinOS.Integrations.FdtPartnerApi.Services;
using SmartCoinOS.Persistence;

namespace SmartCoinOS.ClientPortal.Api;

internal static class ProgramExtensions
{
    internal static void AddHttpClients(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddGraphApiClient(builder.Configuration.GetRequiredSection(nameof(GraphApiConfig)).Bind)
            .AddFdtPartnerApi(builder.Configuration.GetRequiredSection(nameof(FdtPartnerSettings)).Bind);
    }

    internal static void AddApplication(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(ProgramExtensions).Assembly);

            config.AddOpenBehavior(typeof(QueryCachingPipelineBehavior<,>));
            config.AddOpenBehavior(typeof(EntityVersionConcurrencyBehavior<,>));
            config.AddOpenBehavior(typeof(CommandValidationBehavior<,>));
            config.AddOpenBehavior(typeof(TransactionCommandBehavior<,>));
        });
        builder.Services.AddValidatorsFromAssembly(AssemblyReference.Assembly, includeInternalTypes: true);
    }

    public static void AddDomainCommandHandlers(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(opts =>
        {
            opts.RegisterServicesFromAssemblyContaining<CommandsAssemblyResolver>();
        });
    }

    internal static void AddApiDocs(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApiDocument(document =>
        {
            document.Title = "SmartCoinOS API - Client Portal";
            document.SchemaSettings.GenerateEnumMappingDescription = true;

            document.PostProcess = d =>
            {
                d.Info.TermsOfService = "Term of service";
                d.Info.Version = "v1.0";
                d.Info.License = new OpenApiLicense
                {
                    Url = "http://www.apache.org/licenses/LICENSE-2.0.html", Name = "Apache 2.0"
                };
            };
            document.AddSecurity("openId", Enumerable.Empty<string>(), new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.OpenIdConnect,
                OpenIdConnectUrl = "https://localhost/.well-known/openid-configuration",
                In = OpenApiSecurityApiKeyLocation.Header,
                Description = "Standard authorisation using the Bearer scheme. Example: \"bearer {token}\"",
            });
        });
    }

    internal static void AddAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(options =>
            {
                builder.Configuration.Bind("IdentityConfigAzureAd", options);
                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters.NameClaimType = ApplicationConstants.Claims.Name;
                options.TokenValidationParameters.ValidateAudience = false;


                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async ctx =>
                    {
                        var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        if (ctx.Principal is null)
                        {
                            ctx.Fail("Not possible to detect claims context.");
                            logger.LogError("Not possible to detect claims context.");
                            return;
                        }

                        var userId = ctx.Principal.FindFirstValue(ApplicationConstants.Claims.UserId);

                        var context = ctx.HttpContext.RequestServices.GetRequiredService<ReadOnlyDataContext>();
                        var clientId = await context.AuthorizedUsers
                            .Where(x => x.ExternalId == userId)
                            .Select(x => x.ClientId)
                            .FirstOrDefaultAsync(ctx.Request.HttpContext.RequestAborted);

                        if (clientId == new ClientId(Guid.Empty))
                        {
                            logger.LogError("Couldn't connect this principal to a valid client.");
                            ctx.Fail("Couldn't connect this principal to a valid client.");
                            return;
                        }
                        else
                        {
                            logger.LogInformation("Client found: {clientId}", clientId);
                        }

                        var claims = new List<Claim> { new(ApplicationConstants.Claims.ClientId, clientId.ToString()) };

                        ctx.Principal.AddIdentity(new ClaimsIdentity(claims));
                    }
                };
            }, options => builder.Configuration.Bind("IdentityConfigAzureAd", options));
    }

    internal static void AddAuthorization(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization();
    }

    internal static void AddCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin();
                policy.AllowAnyMethod();
                policy.AllowAnyHeader();
            });
        });
    }

    internal static void AddErrorHandlers(this WebApplicationBuilder builder)
    {
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    }

    internal static void AddJsonOptions(this WebApplicationBuilder builder)
    {
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(
                new JsonStringEnumConverter(namingPolicy: JsonNamingPolicy.KebabCaseLower, allowIntegerValues: false));
        });

        builder.Services.AddControllers().AddJsonOptions(jsonOptions =>
        {
            jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
    }

    internal static void AddValidationSchemaProvider(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton(provider =>
        {
            var schemaProvider = new ValidationSchemaProvider(provider);
            schemaProvider.InitializeValidationSchemas();
            return schemaProvider;
        });

        ValidatorOptions.Global.LanguageManager = new ValidationLanguageManager();

        builder.Services.AddOutputCache();
    }

    internal static void AddAwsServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddAwsServiceProvider(
            builder.Configuration.GetRequiredSection(nameof(AwsS3Configuration)).Bind);
    }
}
