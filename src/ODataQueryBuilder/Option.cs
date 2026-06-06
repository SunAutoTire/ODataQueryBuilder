namespace SunAuto.OData;

/// <summary>
/// Base class for OData query clauses (e.g. <c>$select</c>, <c>$expand</c>).
/// </summary>
public abstract class Option
{
    /// <param name="arguments">The clause values to include in the query string.</param>
    public Option(params object[] arguments)
    {
    }

    // /// <summary>/ Initializes a new instance of the <see cref="Option"/> class with the specified arguments and an optional nested clause.</summary>
    // /// <param name="argument">The clause value to include in the query string.</param>
    // /// <param name="nestedClause">An optional nested clause that applies to this clause (e.g. a <c>$select</c> clause nested within an <c>$expand</c> clause).</param>
    // public Option(string argument, Option? nestedClause) : this(argument)
    // {
    //     NestedClause = nestedClause;
    // }

    /// <summary>Gets the OData clause keyword (e.g. <c>select</c>, <c>expand</c>).</summary>
    protected abstract string Name { get; }

    // /// <summary>Gets the values that make up this clause.</summary>
    // public object[] Values { get; } = arguments;

    /// <summary>
    /// Gets an optional nested clause that applies to this clause (e.g. a <c>$select</c> clause nested within an <c>$expand</c> clause).
    /// </summary>
    protected IEnumerable<OptionValue> OptionValues { get; set; } = [];

    public override string ToString() => $"${Name}={string.Join(',', OptionValues.Select(ov => ov.ToString()))}";
}