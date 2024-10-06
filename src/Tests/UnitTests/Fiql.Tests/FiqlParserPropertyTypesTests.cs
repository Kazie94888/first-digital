using SmartCoinOS.Extensions.Fiql;

namespace Fiql.Tests;
public sealed class FiqlParserPropertyTypesTests
{
    [Fact]
    public void ParseToExpression_NullableIntProperty_HandlesNullComparison()
    {
        const string filter = "nullableAge=is-null="; // 'nullableAge' is an int? that could be null

        var expression = FiqlParser.ParseToExpression<TestEntity>(filter);
        var func = expression.Compile();

        Assert.True(func(new TestEntity { NullableAge = null }));
        Assert.False(func(new TestEntity { NullableAge = 25 }));
    }

    [Fact]
    public void ParseToExpression_ComplexObjectProperty_AccessNestedProperty()
    {
        const string filter = "address.city==Springfield"; // 'address' is a complex object with a 'city' property

        var expression = FiqlParser.ParseToExpression<TestEntity>(filter);
        var func = expression.Compile();

        Assert.True(func(new TestEntity { Address = new Address { City = "Springfield" } }));
        Assert.False(func(new TestEntity { Address = new Address { City = "Shelbyville" } }));
    }

    private class TestEntity
    {
        public string? Name { get; set; }
        public int? NullableAge { get; set; }
        public List<string>? Tags { get; set; }
        public Address? Address { get; set; }
        public List<Address>? Addresses { get; set; }
    }

    private class Address
    {
        public string? Street { get; set; }
        public string? City { get; set; }
    }
}