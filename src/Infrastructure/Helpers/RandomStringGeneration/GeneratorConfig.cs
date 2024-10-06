namespace SmartCoinOS.Infrastructure.Helpers.RandomStringGeneration;

public readonly record struct GeneratorConfig
{
    public GeneratorConfig() : this(length: 8, lowerCase: true, upperCase: true, digits: true, characters: true)
    {
    }

    public GeneratorConfig(int length, bool lowerCase, bool upperCase, bool digits, bool characters)
    {
        Length = length;
        LowerCase = lowerCase;
        UpperCase = upperCase;
        Digits = digits;
        Characters = characters;
    }

    public readonly int Length;
    public readonly bool LowerCase;
    public readonly bool UpperCase;
    public readonly bool Digits;
    public readonly bool Characters;
}