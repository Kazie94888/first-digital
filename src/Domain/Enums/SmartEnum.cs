namespace SmartCoinOS.Domain.Enums;

internal sealed record SmartEnum(string Name)
{
    public override string ToString() => Name;
    public static implicit operator string(SmartEnum smartEnum) => smartEnum.Name;
}

internal abstract record SmartEnumCollection
{
    protected SmartEnumCollection(string typeName, List<string> items)
    {
        TypeName = typeName;
        Items = items.Select(x => new SmartEnum(x)).ToList();
    }

    internal string TypeName { get; }
    internal IReadOnlyList<SmartEnum> Items { get; }

    internal bool Found(string key) => Items.Any(x => x.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
}