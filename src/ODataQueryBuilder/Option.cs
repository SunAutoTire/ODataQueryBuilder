namespace SunAuto.OData;

/// <summary>
/// Base class for OData query clauses (e.g. <c>$select</c>, <c>$expand</c>).
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Option"/> class with the specified clause values.
/// </remarks>
/// <param name="optionValues">The clause values to include in the query string.</param>
public abstract class Option(params object[] optionValues)
{
    /// <summary>
    /// Gets the OData clause keyword (e.g. <c>select</c>, <c>expand</c>).
    /// </summary>
    protected abstract string Name { get; }

    protected virtual string Suffix => string.Empty;

    // /// <summary>Gets the values that make up this clause.</summary>
    // public object[] Values { get; } = arguments;

    /// <summary>
    /// Gets an optional nested clause that applies to this clause (e.g. a <c>$select</c> clause nested within an <c>$expand</c> clause).
    /// </summary>
    protected IEnumerable<OptionValue> OptionValues { get; set; } = [.. optionValues
            .Select(ov => ov is OptionValue ov2
                ? ov2 :
                new OptionValue(ov.ToString() ?? string.Empty))];

    /// <summary>
    /// Returns a string representation of the OData clause, including its keyword and values, formatted according to OData query syntax (e.g. <c>$select=Id,Name</c> or <c>$expand=Orders($select=Id,Total)</c>).
    /// </summary>
    /// <returns>A string representation of the OData clause.</returns>
    public override string ToString() => OptionValues.Any() 
        ? $"${Name}={string.Join(',', OptionValues.Select(ov => ov.ToString()))}{Suffix}" 
        : $"${Name}{Suffix}";

    internal Option Add(Option option)
    {
        OptionValues = [.. OptionValues, new OptionValue(string.Empty, option)];
        return option;
    }
}