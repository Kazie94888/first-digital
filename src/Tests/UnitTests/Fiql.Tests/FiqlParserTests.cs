using SmartCoinOS.Extensions.Fiql;

namespace Fiql.Tests;

public sealed class FiqlParserTests
{
    [Fact]
    public void ParseToExpression_EqualOperator_ReturnsCorrectExpression()
    {
        const string filter = "name==John";

        var expression = FiqlParser.ParseToExpression<TestEntity>(filter);
        var func = expression.Compile();
        var result = func(new TestEntity { Name = "John" });

        Assert.True(result);
    }

    [Fact]
    public void ParseToExpression_InvalidOperator_ThrowsInvalidOperationException()
    {
        const string filter = "name=abc"; // Invalid operator

        Assert.Throws<InvalidOperationException>(() => FiqlParser.ParseToExpression<TestEntity>(filter));
    }

    [Fact]
    public void ParseToExpression_InOperator_ReturnsCorrectExpression()
    {
        const string filter = "id=in=(1,2,3)";
        var expectedIds = new List<int> { 1, 2, 3 };

        var expression = FiqlParser.ParseToExpression<TestEntity>(filter);
        var func = expression.Compile();

        Assert.Contains(expectedIds, id => func(new TestEntity { Id = id }));
        Assert.False(func(new TestEntity { Id = 4 })); // An ID not in the list
    }

    [Fact]
    public void ParseToExpression_AndOrConditions_ReturnsCorrectExpression()
    {
        const string filter = "(name==John;age=gt=30),name!=Jane";
        // Expected logic: (name == "John" AND age > 30) OR name != "Jane"

        var expression = FiqlParser.ParseToExpression<TestEntity>(filter);
        var func = expression.Compile();

        Assert.True(func(new TestEntity { Name = "John", Age = 31 }));
        Assert.False(func(new TestEntity { Name = "Jane", Age = 25 }));
    }

    [Fact]
    public void ParseToExpression_ComplexCondition_ReturnsCorrectExpression()
    {
        const string filter = "(name==John;(age=gt=30,Id=lt=10)),name!=Jane";
        // Expected logic: (name == "John" AND (age > 30 OR Id < 10)) OR name != "Jane"

        var expression = FiqlParser.ParseToExpression<TestEntity>(filter);
        var func = expression.Compile();

        Assert.True(func(new TestEntity { Name = "John", Age = 31 }));
        Assert.False(func(new TestEntity { Name = "Jane", Age = 25 }));
        Assert.True(func(new TestEntity { Name = "John", Age = 2, Id = 2 }));
    }

    private class TestEntity
    {
        public string? Name { get; set; }
        public int Age { get; set; }
        public int Id { get; set; }
    }
}
