namespace SunAuto.OData;

/// <summary>
/// Extension methods that scope query options to a single value, producing the nested form
/// <c>Property($expand=Child;$orderby=Name desc)</c> used inside <c>$select</c> and <c>$expand</c>.
/// </summary>
/// <remarks>
/// Each call appends another nested option, so chaining builds them up in call order. The <c>string</c>
/// overloads start a chain from a property name; the <see cref="OptionValue"/> overloads continue one.
/// </remarks>
public static class Extensions
{
    /// <summary>Scopes a <c>$select</c> to a property.</summary>
    /// <param name="property">The property the nested option applies to.</param>
    /// <param name="optionValues">The properties to select within <paramref name="property"/>.</param>
    public static OptionValue Select(this string property, params OptionValue[] optionValues)
        => new OptionValue(property).Select(optionValues);

    /// <inheritdoc cref="Select(string, OptionValue[])" />
    /// <param name="target">The value the nested option applies to.</param>
    /// <param name="optionValues">The properties to select within <paramref name="target"/>.</param>
    public static OptionValue Select(this OptionValue target, params OptionValue[] optionValues)
        => target.Nest("select", optionValues);

    /// <summary>Scopes an <c>$expand</c> to a property.</summary>
    /// <param name="property">The navigation property the nested option applies to.</param>
    /// <param name="optionValues">The navigation properties to expand within <paramref name="property"/>.</param>
    public static OptionValue Expand(this string property, params OptionValue[] optionValues)
        => new OptionValue(property).Expand(optionValues);

    /// <inheritdoc cref="Expand(string, OptionValue[])" />
    /// <param name="target">The value the nested option applies to.</param>
    /// <param name="optionValues">The navigation properties to expand within <paramref name="target"/>.</param>
    public static OptionValue Expand(this OptionValue target, params OptionValue[] optionValues)
        => target.Nest("expand", optionValues);

    /// <summary>Scopes a <c>$filter</c> to a property.</summary>
    /// <param name="property">The navigation property the nested option applies to.</param>
    /// <param name="optionValues">The filter expressions to apply within <paramref name="property"/>.</param>
    public static OptionValue Filter(this string property, params OptionValue[] optionValues)
        => new OptionValue(property).Filter(optionValues);

    /// <inheritdoc cref="Filter(string, OptionValue[])" />
    /// <param name="target">The value the nested option applies to.</param>
    /// <param name="optionValues">The filter expressions to apply within <paramref name="target"/>.</param>
    public static OptionValue Filter(this OptionValue target, params OptionValue[] optionValues)
        => target.Nest("filter", optionValues);

    /// <summary>Scopes an ascending <c>$orderby</c> to a property.</summary>
    /// <param name="property">The navigation property the nested option applies to.</param>
    /// <param name="optionValues">The properties to order by within <paramref name="property"/>.</param>
    public static OptionValue OrderBy(this string property, params OptionValue[] optionValues)
        => new OptionValue(property).OrderBy(optionValues);

    /// <inheritdoc cref="OrderBy(string, OptionValue[])" />
    /// <param name="target">The value the nested option applies to.</param>
    /// <param name="optionValues">The properties to order by within <paramref name="target"/>.</param>
    public static OptionValue OrderBy(this OptionValue target, params OptionValue[] optionValues)
        => target.Nest("orderby", optionValues);

    /// <summary>Scopes a descending <c>$orderby</c> to a property.</summary>
    /// <param name="property">The navigation property the nested option applies to.</param>
    /// <param name="optionValues">The properties to order by within <paramref name="property"/>.</param>
    public static OptionValue OrderByDescending(this string property, params OptionValue[] optionValues)
        => new OptionValue(property).OrderByDescending(optionValues);

    /// <inheritdoc cref="OrderByDescending(string, OptionValue[])" />
    /// <param name="target">The value the nested option applies to.</param>
    /// <param name="optionValues">The properties to order by within <paramref name="target"/>.</param>
    public static OptionValue OrderByDescending(this OptionValue target, params OptionValue[] optionValues)
        => target.Nest("orderby", Descending(optionValues));

    /// <summary>Appends <c>desc</c> to each value, as <c>$orderby</c> requires for descending sorts.</summary>
    internal static OptionValue[] Descending(IEnumerable<OptionValue> optionValues)
        => [.. optionValues.Select(ov => new OptionValue($"{ov} desc"))];

    private static OptionValue Nest(this OptionValue target, string name, params OptionValue[] optionValues)
    {
        target.NestedOptions.Add(new Option(name, optionValues));

        return target;
    }
}
