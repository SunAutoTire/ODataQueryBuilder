namespace SunAuto.OData;

/// <summary>
/// A single value within an OData query option, optionally carrying options scoped to it — for example the
/// <c>$expand=SubProperty</c> in <c>$expand=Property($expand=SubProperty)</c>.
/// </summary>
/// <remarks>
/// Immutable. The extension methods that attach nested options return a new value rather than mutating this
/// one, so an <see cref="OptionValue"/> held in a variable can be used as the base of several different
/// chains without them writing into each other.
/// </remarks>
/// <param name="value">The literal value, typically a property name or an expression fragment.</param>
/// <param name="nestedOptions">Options scoped to <paramref name="value"/>.</param>
public class OptionValue(string value, params Option[] nestedOptions)
{
    /// <summary>Gets the literal value.</summary>
    public string Value { get; } = value;

    /// <summary>Gets the options scoped to <see cref="Value"/>.</summary>
    public IReadOnlyList<Option> NestedOptions { get; } = nestedOptions;

    /// <summary>Converts a string into an <see cref="OptionValue"/> carrying no nested options.</summary>
    public static implicit operator OptionValue(string? value) => new(value ?? string.Empty);

    /// <summary>Converts an expression into an <see cref="OptionValue"/> carrying no nested options.</summary>
    public static implicit operator OptionValue(Expression expression) => new(expression.ToString());

    /// <summary>
    /// Renders the value, appending its nested options in parentheses when present. Nested options are
    /// separated by <c>;</c> as required by the OData ABNF.
    /// </summary>
    public override string ToString() => NestedOptions.Count > 0
        ? $"{Value}({string.Join(';', NestedOptions)})"
        : Value;
}
