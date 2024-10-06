using SmartCoinOS.Domain.Shared;

namespace SmartCoinOS.Domain.Orders;

public record OrderNumber
{
    public required string Value { get; init; }

    public static implicit operator string(OrderNumber orderNumber) => orderNumber.Value;
    public override string ToString() => Value;

    public static OrderNumber New(OrderType orderType)
    {
        var randomString = RandomGenerator.GenerateRandomString(length: 5, lowerCase: false, upperCase: true, digits: true, requireDiversity: false);
        var firstPart = randomString[..2];
        var lastPart = randomString[3..];
        var draftOrderNumber = $"{firstPart}-{lastPart}";

        var checkDigit = ChecksumCalculator.CalculateCheckDigit(draftOrderNumber);
        var prefix = orderType == OrderType.Mint ? 'M' : 'R';
        var orderNumber = $"{prefix}{draftOrderNumber}{checkDigit}";

        return new OrderNumber
        {
            Value = orderNumber
        };
    }

    public static bool IsValid(OrderNumber orderNumber)
    {
        var order = orderNumber.Value;

        if (order[0] is not 'M' and not 'R')
            return false;

        if (!char.IsDigit(order[^1]))
            return false;

        var valueToCheck = order[1..^1];
        var checkDigit = ChecksumCalculator.CalculateCheckDigit(valueToCheck);

        return Math.Abs(char.GetNumericValue(order[^1]) - checkDigit) < double.Epsilon;
    }
}
