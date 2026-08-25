namespace SunAuto.OData;

/// <summary>
/// A single OData query option: a keyword and the values that follow it, e.g. <c>$select=Id,Name</c>.
/// </summary>
public sealed class Option
{
    private readonly List<OptionValue> optionValues;

    /// <summary>Creates an option.</summary>
    /// <param name="name">The option keyword without its <c>$</c> prefix.</param>
    /// <param name="optionValues">The values that make up the option.</param>
    public Option(string name, params OptionValue[] optionValues)
    {
        Name = name;
        this.optionValues = [.. optionValues];
    }

    /// <summary>Gets the option keyword without its <c>$</c> prefix (e.g. <c>select</c>).</summary>
    public string Name { get; }

    /// <summary>Gets the values that make up this option.</summary>
    public IReadOnlyList<OptionValue> OptionValues => optionValues;

    /// <summary>Renders the option in OData query syntax.</summary>
    public override string ToString() => optionValues.Count > 0
        ? $"${Name}={string.Join(',', optionValues)}"
        : $"${Name}";

    internal void Add(OptionValue optionValue) => optionValues.Add(optionValue);

    internal void Set(OptionValue optionValue)
    {
        optionValues.Clear();
        optionValues.Add(optionValue);
    }
}
