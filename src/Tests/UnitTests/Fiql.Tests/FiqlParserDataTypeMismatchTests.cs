using SmartCoinOS.Extensions.Fiql;

namespace Fiql.Tests;

public sealed class FiqlParserDataTypeMismatchTests
{
    [Fact]
    public void ParseToExpression_IntPropertyWithStringValue_ThrowsInvalidOperationException()
    {
        const string filter = "age==twenty"; // 'age' expects an integer, but 'twenty' is a string

        var exception = Assert.Throws<FormatException>(() => FiqlParser.ParseToExpression<TestEntity>(filter));
        Assert.Contains("The input string 'twenty' was not in a correct format", exception.Message);
    }

    [Fact]
    public void ParseToExpression_DatePropertyWithInvalidFormat_ThrowsInvalidOperationException()
    {
        const string filter = "birthdate==2022-02-30"; // Invalid date format/value

        var exception = Assert.Throws<InvalidOperationException>(() => FiqlParser.ParseToExpression<TestEntity>(filter));
        Assert.Contains("Cannot parse 2022-02-30 into System.DateTime", exception.Message);
    }

    [Fact]
    public void ParseToExpression_BooleanPropertyWithNonBooleanValue_ThrowsInvalidOperationException()
    {
        const string filter = "isActive==yes"; // 'isActive' expects a boolean, 'yes' is not a boolean

        var exception = Assert.Throws<FormatException>(() => FiqlParser.ParseToExpression<TestEntity>(filter));
        Assert.Contains("String 'yes' was not recognized as a valid Boolean", exception.Message);
    }

    [Fact]
    public void ParseToExpression_EnumPropertyWithInvalidValue_ThrowsInvalidOperationException()
    {
        const string filter = "status==InvalidStatus"; // Assuming 'status' is an enum and 'InvalidStatus' is not a valid enum value

        Assert.Throws<ArgumentException>(() => FiqlParser.ParseToExpression<TestEntity>(filter));
    }

    private class TestEntity
    {
        public string? Name { get; set; }
        public int Age { get; set; }
        public DateTime Birthdate { get; set; }
        public bool IsActive { get; set; }
        public Status Status { get; set; }
    }

    private enum Status
    {
        Active,
        Inactive,
        Pending
    }
}