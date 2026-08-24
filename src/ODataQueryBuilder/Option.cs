namespace SunAuto.OData;

/// <summary>
/// A single OData query option: a keyword and the values that follow it, e.g. <c>$select=Id,Name</c>.
/// </summary>
/// <param name="name">The option keyword without its <c>$</c> prefix.</param>
/// <param name="optionValues">The values that make up the option.</param>
public sealed class Option(string name, params OptionValue[] optionValues)
{
    /// <summary>Gets the option keyword without its <c>$</c> prefix (e.g. <c>select</c>).</summary>
    public string Name { get; } = name;

    /// <summary>Gets the values that make up this option.</summary>
    public IList<OptionValue> OptionValues { get; } = [.. optionValues];

    /// <summary>Renders the option in OData query syntax.</summary>
    public override string ToString() => OptionValues.Count > 0
        ? $"${Name}={string.Join(',', OptionValues)}"
        : $"${Name}";
}
