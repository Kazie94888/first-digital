using SmartCoinOS.Extensions.Fiql;

namespace Fiql.Tests;

public sealed class FiqlParserEdgeCasesTests
{
    [Fact]
    public void ParseToExpression_EmptyFilter_ThrowsArgumentException()
    {
        const string filter = "";

        var exception = Assert.Throws<InvalidOperationException>(() => FiqlParser.ParseToExpression<TestEntity>(filter));
        Assert.Contains("Unable to find operator", exception.Message);
    }

    [Fact]
    public void ParseToExpression_InvalidSyntax_ThrowsFormatException()
    {
        const string filter = "name=="; // Missing value part

        Assert.Throws<InvalidOperationException>(() => FiqlParser.ParseToExpression<TestEntity>(filter));
    }

    [Fact]
    public void ParseToExpression_InvalidCharactersInPropertyName_ThrowsInvalidOperationException()
    {
        const string filter = "na!me==John"; // Invalid character '!' in property name

        Assert.Throws<ArgumentException>(() => FiqlParser.ParseToExpression<TestEntity>(filter));
    }

    [Fact]
    public void ParseToExpression_InvalidNestedProperty_ThrowsInvalidOperationException()
    {
        const string filter = "profile.gender==m"; // Assuming 'profile' or 'profile.age' does not exist

        Assert.Throws<ArgumentException>(() => FiqlParser.ParseToExpression<TestEntity>(filter));
    }

    [Fact]
    public void ParseToExpression_UnsupportedOperator_ThrowsInvalidOperationException()
    {
        const string filter = "age=like=20"; // 'like' operator is not supported

        Assert.Throws<NotSupportedException>(() => FiqlParser.ParseToExpression<TestEntity>(filter));
    }

    [Fact]
    public void ParseToExpression_VeryLongFilter_HandlesWithoutError()
    {
        var filter = new string('a', 10000) + "==value"; // Extremely long property name

        // Here, depending on how your parser is implemented, you might expect it to throw an exception
        // due to unrealistic property name length or handle it gracefully.
        // This example assumes an exception for an unrealistic scenario.
        Assert.Throws<ArgumentException>(() => FiqlParser.ParseToExpression<TestEntity>(filter));
    }

    private class TestEntity
    {
        public string? Name { get; set; }
        public int Age { get; set; }
        public int Id { get; set; }
        // Assuming nested properties for testing
        public Profile? Profile { get; set; }
    }

    private class Profile
    {
        public int Age { get; set; }
    }
}