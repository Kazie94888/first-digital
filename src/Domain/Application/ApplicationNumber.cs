using SmartCoinOS.Domain.Shared;

namespace SmartCoinOS.Domain.Application;

public sealed record ApplicationNumber
{
    public required string Value { get; init; }

    public static implicit operator string(ApplicationNumber applicationNumber) => applicationNumber.Value;

    public static ApplicationNumber New()
    {
        var draftApplicationNumber = RandomGenerator.GenerateRandomString(length: 8, lowerCase: false, upperCase: true, digits: true, requireDiversity: false);
        var checkDigit = ChecksumCalculator.CalculateCheckDigit(draftApplicationNumber);
        var applicationNumber = $"{draftApplicationNumber}{checkDigit}";

        return new ApplicationNumber
        {
            Value = applicationNumber
        };
    }

    public static bool IsValid(ApplicationNumber applicationNumber)
    {
        var appNumber = applicationNumber.Value;
        var lastChar = appNumber[^1];
        if (!char.IsDigit(lastChar))
            return false;

        var valueToCheck = appNumber[..^1];
        var checkDigit = ChecksumCalculator.CalculateCheckDigit(valueToCheck);

        return Math.Abs(char.GetNumericValue(lastChar) - checkDigit) < double.Epsilon;
    }
}