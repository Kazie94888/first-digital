using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using FluentValidation;
using FluentValidation.Validators;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Extensions;

namespace SmartCoinOS.ClientPortal.Api.Features.ValidationSchema;

internal sealed class ValidationSchemaProvider
{
    private sealed class ValidationSchema
    {
        public required JsonObject Schema { get; init; }
        public required string Hash { get; init; }
    }

    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, ValidationSchema> _schemas = [];

    private static readonly Type[] _applicableValidators =
    [
        typeof(AspNetCoreCompatibleEmailValidator<>),
        typeof(EmptyValidator<,>),
        typeof(EnumValidator<,>),
        typeof(ExclusiveBetweenValidator<,>),
        typeof(GreaterThanOrEqualValidator<,>),
        typeof(GreaterThanValidator<,>),
        typeof(InclusiveBetweenValidator<,>),
        typeof(LengthValidator<>),
        typeof(LessThanOrEqualValidator<,>),
        typeof(LessThanValidator<,>),
        typeof(NotEmptyValidator<,>),
        typeof(NotEqualValidator<,>),
        typeof(NotNullValidator<,>),
        typeof(NullValidator<,>),
        typeof(RangeValidator<,>),
        typeof(RegularExpressionValidator<>),
        typeof(ScalePrecisionValidator<>),
        typeof(StringEnumValidator<>)
    ];

    public ValidationSchemaProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void InitializeValidationSchemas()
    {
        using var scope = _serviceProvider.CreateScope();
        var commands = AssemblyReference.Assembly.GetTypes()
            .Where(x => x.IsAssignableTo(typeof(IBaseCommand)))
            .ToArray();

        var method = GetType().GetMethod(nameof(GetValidationSchemaInternal),
            BindingFlags.Static | BindingFlags.NonPublic)!;

        foreach (var command in commands)
        {
            var genericMethod = method.MakeGenericMethod(command);
            var schema = (JsonObject)genericMethod.Invoke(this, [scope.ServiceProvider])!;

            var camelCaseName = command.Name.ToCamelCase();
            _schemas[camelCaseName] = new ValidationSchema
            {
                Schema = schema,
                Hash = GetHash(schema.ToJsonString())
            };
        }
    }

    public (string JsonSchema, string Hash) GetValidationSchema(string commandName)
    {
        var schema = _schemas[commandName];
        var commandSchemaObject = new JsonObject { [commandName] = schema.Schema.DeepClone() };
        return (commandSchemaObject.ToJsonString(), schema.Hash);
    }

    private static JsonObject GetValidationSchemaInternal<TCommand>(IServiceProvider provider) where TCommand : IBaseCommand
    {
        var commandValidators = provider.GetServices<IValidator<TCommand>>();
        var commandObject = new JsonObject();
        foreach (var commandValidator in commandValidators)
        {
            var descriptor = commandValidator.CreateDescriptor();
            var propertyValidators = descriptor.GetMembersWithValidators()
                .Select(x =>
                (
                    PropertyName: x.Key,
                    ValidatorsAndOptions: x.AsEnumerable()
                ));

            foreach (var (fullPropertyName, validatorsAndOptions) in propertyValidators)
            {
                var firstLevelName = fullPropertyName.ToCamelCase();

                var propertyObject = new JsonObject();
                var rulesObject = new JsonObject();
                foreach (var (validator, options) in validatorsAndOptions)
                {
                    var validatorName = validator.Name.Replace("Validator", "");
                    var validatorType = validator.GetType();
                    if (!IsValidatorApplicable(validatorType))
                        continue;

                    var innerMostPropName = firstLevelName.Split(".")[^1];
                    var validatorMessage = options.GetUnformattedErrorMessage();
                    var errorMessage = ReplacePlaceholders(validator, validatorMessage, innerMostPropName);

                    var ruleObject = new JsonObject
                    {
                        ["message"] = errorMessage
                    };
                    SetValidatorValues(ruleObject, validator, validatorType);
                    rulesObject[validatorName.ToCamelCase()] = ruleObject;
                }

                if (rulesObject.Count != 0)
                    propertyObject["rules"] = rulesObject;

                if (propertyObject.Count != 0)
                    commandObject[firstLevelName] = propertyObject;
            }
        }

        return commandObject;
    }

    private static string ReplacePlaceholders(IPropertyValidator validator, string unformattedMessage, string propertyName)
    {
        var errorMessage = unformattedMessage.Replace("'{PropertyName}'", propertyName);
        switch (validator)
        {
            case ILengthValidator lengthValidator:
                errorMessage = errorMessage.Replace("{MinLength}", $"{lengthValidator.Min}");
                errorMessage = errorMessage.Replace("{MaxLength}", $"{lengthValidator.Max}");
                break;
            case IComparisonValidator compValidator:
                errorMessage = errorMessage.Replace("{ComparisonValue}", $"{compValidator.ValueToCompare}");
                break;
            case IBetweenValidator betweenValidator:
                errorMessage = errorMessage.Replace("{From}", $"{betweenValidator.From}");
                errorMessage = errorMessage.Replace("{To}", $"{betweenValidator.To}");
                break;
        }

        return errorMessage;
    }

    private static void SetValidatorValues(JsonNode propertyObject, IPropertyValidator validator, Type validatorType)
    {
        switch (validator)
        {
            case IBetweenValidator betweenValidator:
                propertyObject["from"] = betweenValidator.From.ToString()!;
                propertyObject["to"] = betweenValidator.To.ToString()!;
                break;
            case IComparisonValidator comparisonValidator:
                propertyObject["value"] = comparisonValidator.ValueToCompare.ToString();
                break;
            case ILengthValidator lengthValidator:
                propertyObject["min"] = lengthValidator.Min;
                propertyObject["max"] = lengthValidator.Max;
                break;
            case IRegularExpressionValidator regularExpressionValidator:
                propertyObject["pattern"] = regularExpressionValidator.Expression;
                break;
        }

        var genericValidatorType = validatorType.GetGenericTypeDefinition();
        if (genericValidatorType == typeof(ScalePrecisionValidator<>))
        {
            propertyObject["scale"] = validatorType.GetProperty("Scale")!.GetValue(validator)!.ToString();
            propertyObject["scale"] = validatorType.GetProperty("Precision")!.GetValue(validator)!.ToString();
        }
    }

    private static bool IsValidatorApplicable(Type validatorType)
    {
        if (!validatorType.IsGenericType)
            return false;

        var genericType = validatorType.GetGenericTypeDefinition();
        return _applicableValidators.Contains(genericType);
    }

    private static string GetHash(string s)
    {
        return BitConverter.ToString(SHA256.HashData(Encoding.UTF8.GetBytes(s)))
            .Replace("-", "")
            .ToLower();
    }
}
