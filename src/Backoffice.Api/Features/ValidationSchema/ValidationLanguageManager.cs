namespace SmartCoinOS.Backoffice.Api.Features.ValidationSchema;

public sealed class ValidationLanguageManager : FluentValidation.Resources.LanguageManager
{
    private const string _supportedLanguage = "en";

    public ValidationLanguageManager()
    {
        Culture = new(_supportedLanguage);
        AddEnglishTranslation();
    }

    private void AddEnglishTranslation()
    {
        AddTranslation(_supportedLanguage, "LengthValidator", "'{PropertyName}' must be between {MinLength} and {MaxLength} characters.");
        AddTranslation(_supportedLanguage, "MinimumLengthValidator", "The length of '{PropertyName}' must be at least {MinLength} characters.");
        AddTranslation(_supportedLanguage, "MaximumLengthValidator", "The length of '{PropertyName}' must be {MaxLength} characters or fewer.");
        AddTranslation(_supportedLanguage, "ExactLengthValidator", "'{PropertyName}' must be {MaxLength} characters in length.");
        AddTranslation(_supportedLanguage, "InclusiveBetweenValidator", "'{PropertyName}' must be between {From} and {To}.");
        AddTranslation(_supportedLanguage, "ExclusiveBetweenValidator", "'{PropertyName}' must be between {From} and {To} (exclusive).");
        AddTranslation(_supportedLanguage, "ScalePrecisionValidator", "'{PropertyName}' must not be more than {ExpectedPrecision} digits in total, with allowance for {ExpectedScale} decimals.");
    }
}