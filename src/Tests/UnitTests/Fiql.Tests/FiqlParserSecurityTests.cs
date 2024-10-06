using SmartCoinOS.Extensions.Fiql;

namespace Fiql.Tests;

public sealed class FiqlParserSecurityTests
{
    [Fact]
    public void ParseToExpression_PreventsInjection_Attacks()
    {
        // Attempt to inject a malicious expression
        const string filter = "name=='); DROP TABLE Students;--";

        // Expecting the parser to either throw a specific exception indicating a syntax error
        // or to safely parse the input without executing any unintended commands.
        // The exact assertion depends on how your parser handles invalid syntax and potential security risks.
        var exception = Record.Exception(() => FiqlParser.ParseToExpression<TestEntity>(filter));

        // Here, you might assert that an exception is thrown, indicating a syntax error.
        // Alternatively, if your parser is designed to ignore or safely handle such input, you might assert no exception is thrown.
        // This example assumes an exception is expected for invalid syntax.
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception); // Assuming InvalidOperationException is thrown for invalid syntax
    }

    [Fact]
    public void ParseToExpression_SafeAgainstSqlInjection_LikePatterns()
    {
        const string filter = "name==Robert'); DROP TABLE Students;--";

        var exception = Assert.Throws<InvalidOperationException>(() => FiqlParser.ParseToExpression<TestEntity>(filter));

        // Optionally check part of the exception message if specific feedback is important
        Assert.Contains("Unable to find operator", exception.Message);

    }

    private class TestEntity
    {
        public string? Name { get; set; }
    }
}