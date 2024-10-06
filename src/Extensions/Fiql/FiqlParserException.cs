namespace SmartCoinOS.Extensions.Fiql;

public sealed class FiqlParserException : Exception
{
    internal FiqlParserException(string message) : base(message)
    {
    }

    public static void ThrowIf(bool condition, string errorMessage)
    {
        if (condition)
            throw new FiqlParserException(errorMessage);
    }
}