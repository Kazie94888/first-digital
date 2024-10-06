using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using AspNetCore.Authentication.ApiKey;
using AWSServiceProvider;
using AWSServiceProvider.Services.S3;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using NSwag;
using SmartCoinOS.Backoffice.Api.Application.Behaviors;
using SmartCoinOS.Backoffice.Api.Base;
using SmartCoinOS.Backoffice.Api.Base.Options;
using SmartCoinOS.Backoffice.Api.Base.WebHooks;
using SmartCoinOS.Backoffice.Api.Features.ValidationSchema;
using SmartCoinOS.Backoffice.Api.Features.Webhooks.SmartTrust;
using SmartCoinOS.Backoffice.Api.Infrastructure;
using SmartCoinOS.Commands;
using SmartCoinOS.Integrations.AzureGraphApi.DependencyInjection;
using SmartCoinOS.Integrations.AzureGraphApi.Options;
using SmartCoinOS.Integrations.CourierApi.DependencyInjection;
using SmartCoinOS.Integrations.FdtPartnerApi.DependencyInjection;
using SmartCoinOS.Integrations.FdtPartnerApi.Services;
using SmartCoinOS.Integrations.GnosisSafeApi.DependencyInjection;

namespace SmartCoinOS.Backoffice.Api;

internal static class ProgramExtensions
{
    public static void AddApplication(this WebApplicationBuilder builder)
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

    public static void AddDomainEventHandlers(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(opts =>
        {
            opts.RegisterServicesFromAssemblyContaining<CommandsAssemblyResolver>();
        });
    }

    public static void AddWebhooks(this WebApplicationBuilder builder)
    {
        builder.Services.AddWebHook<SmartTrustWebhook>();
    }

    public static void AddValidationSchemaProvider(this WebApplicationBuilder builder)
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

    internal static void AddHttpClients(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddFdtPartnerApi(builder.Configuration.GetRequiredSection(nameof(FdtPartnerSettings)).Bind)
            .AddGnosisApi()
            .AddGraphApiClient(builder.Configuration.GetRequiredSection(nameof(GraphApiConfig)).Bind)
            .AddCourierApi(builder.Configuration.GetRequiredSection(nameof(CourierSettings)).Bind);
    }

    internal static void AddAwsServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddAwsServiceProvider(
            builder.Configuration.GetRequiredSection(nameof(AwsS3Configuration)).Bind);
    }

    internal static void AddApiDocs(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApiDocument(document =>
        {
            document.Title = "SmartCoinOS API - Back Office";
            document.SchemaSettings.GenerateEnumMappingDescription = true;

            document.PostProcess = d =>
            {
                d.Info.TermsOfService = "Term of service";
                d.Info.Version = "v1.0";
                d.Info.License = new OpenApiLicense
                {
                    Url = "http://www.apache.org/licenses/LICENSE-2.0.html",
                    Name = "Apache 2.0"
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

    internal static void AddBlockchainSettings(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<BlockchainSettings>(
            builder.Configuration.GetRequiredSection(nameof(BlockchainSettings)));
    }

    internal static void AddAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<AuthenticationApiKeysConfig>(
            builder.Configuration.GetRequiredSection(nameof(AuthenticationApiKeysConfig)));
        builder.Services.AddSingleton<ApiKeyAuthenticationHandler>();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddApiKeyInHeader(opts =>
            {
                opts.SuppressWWWAuthenticateHeader = true;
                opts.KeyName = "X-Api-Key";

                opts.Events.OnValidateKey = async context =>
                {
                    var handler = context.HttpContext.RequestServices.GetRequiredService<ApiKeyAuthenticationHandler>();
                    await handler.ValidateKeyAsync(context);
                };
            })
            .AddMicrosoftIdentityWebApi(options =>
            {
                builder.Configuration.Bind("IdentityConfigAzureAd", options);
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters.NameClaimType = "name";
            }, options => builder.Configuration.Bind("IdentityConfigAzureAd", options));
    }

    public static void AddAuthorization(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(ApplicationConstants.PolicyNames.SmartTrustAuthPolicy, policy =>
            {
                policy.AddAuthenticationSchemes(ApiKeyDefaults.AuthenticationScheme);
                policy.RequireClaim(ClaimTypes.Spn, ApplicationConstants.PolicyNames.SmartTrustAuthPolicy);
            });
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

    internal static void AddErrorHandlers(this WebApplicationBuilder builder)
    {
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    }
}