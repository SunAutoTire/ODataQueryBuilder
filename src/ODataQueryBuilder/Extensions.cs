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

    /// <summary>Scopes a <c>$compute</c> to a property.</summary>
    /// <param name="property">The navigation property the nested option applies to.</param>
    /// <param name="optionValues">The compute expressions to evaluate within <paramref name="property"/>.</param>
    public static OptionValue Compute(this string property, params OptionValue[] optionValues)
        => new OptionValue(property).Compute(optionValues);

    /// <inheritdoc cref="Compute(string, OptionValue[])" />
    /// <param name="target">The value the nested option applies to.</param>
    /// <param name="optionValues">The compute expressions to evaluate within <paramref name="target"/>.</param>
    public static OptionValue Compute(this OptionValue target, params OptionValue[] optionValues)
        => target.Nest("compute", optionValues);

    /// <summary>Scopes a <c>$top</c> to a property.</summary>
    /// <param name="property">The navigation property the nested option applies to.</param>
    /// <param name="value">The maximum number of items to return.</param>
    public static OptionValue Top(this string property, int value) => new OptionValue(property).Top(value);

    /// <inheritdoc cref="Top(string, int)" />
    /// <param name="target">The value the nested option applies to.</param>
    /// <param name="value">The maximum number of items to return.</param>
    public static OptionValue Top(this OptionValue target, int value) => target.Nest("top", Number(value));

    /// <summary>Scopes a <c>$skip</c> to a property.</summary>
    /// <param name="property">The navigation property the nested option applies to.</param>
    /// <param name="value">The number of items to skip.</param>
    public static OptionValue Skip(this string property, int value) => new OptionValue(property).Skip(value);

    /// <inheritdoc cref="Skip(string, int)" />
    /// <param name="target">The value the nested option applies to.</param>
    /// <param name="value">The number of items to skip.</param>
    public static OptionValue Skip(this OptionValue target, int value) => target.Nest("skip", Number(value));

    /// <summary>
    /// Scopes a <c>$count</c> to a property. The argument is required rather than defaulted, because
    /// <c>"Items".Count()</c> would bind to <see cref="System.Linq.Enumerable.Count{T}(IEnumerable{T})"/> and
    /// silently count the characters of the string instead.
    /// </summary>
    /// <param name="property">The navigation property the nested option applies to.</param>
    /// <param name="value">Whether the service should include the count of the expanded collection.</param>
    public static OptionValue Count(this string property, bool value) => new OptionValue(property).Count(value);

    /// <inheritdoc cref="Count(string, bool)" />
    /// <param name="target">The value the nested option applies to.</param>
    /// <param name="value">Whether the service should include the count of the expanded collection.</param>
    public static OptionValue Count(this OptionValue target, bool value)
        => target.Nest("count", value ? "true" : "false");

    /// <summary>Scopes a <c>$search</c> to a property.</summary>
    /// <param name="property">The navigation property the nested option applies to.</param>
    /// <param name="expression">The free-text search expression.</param>
    public static OptionValue Search(this string property, string expression) => new OptionValue(property).Search(expression);

    /// <inheritdoc cref="Search(string, string)" />
    /// <param name="target">The value the nested option applies to.</param>
    /// <param name="expression">The free-text search expression.</param>
    public static OptionValue Search(this OptionValue target, string expression)
        => target.Nest("search", Expression.Escape(expression));

    /// <summary>Scopes a <c>$levels</c> to a property, expanding a hierarchy to a fixed depth.</summary>
    /// <param name="property">The navigation property the nested option applies to.</param>
    /// <param name="value">How many levels deep to expand.</param>
    public static OptionValue Levels(this string property, int value) => new OptionValue(property).Levels(value);

    /// <inheritdoc cref="Levels(string, int)" />
    /// <param name="target">The value the nested option applies to.</param>
    /// <param name="value">How many levels deep to expand.</param>
    public static OptionValue Levels(this OptionValue target, int value) => target.Nest("levels", Number(value));

    /// <summary>Scopes <c>$levels=max</c> to a property, expanding a hierarchy as deep as the service allows.</summary>
    /// <param name="property">The navigation property the nested option applies to.</param>
    public static OptionValue LevelsMax(this string property) => new OptionValue(property).LevelsMax();

    /// <inheritdoc cref="LevelsMax(string)" />
    /// <param name="target">The value the nested option applies to.</param>
    public static OptionValue LevelsMax(this OptionValue target) => target.Nest("levels", "max");

    private static OptionValue Number(int value) => new(value.ToString(System.Globalization.CultureInfo.InvariantCulture));

    /// <summary>Appends <c>desc</c> to each value, as <c>$orderby</c> requires for descending sorts.</summary>
    internal static OptionValue[] Descending(IEnumerable<OptionValue> optionValues)
        => [.. optionValues.Select(ov => new OptionValue($"{ov} desc"))];

    private static OptionValue Nest(this OptionValue target, string name, params OptionValue[] optionValues)
        => new(target.Value, [.. target.NestedOptions, new Option(name, optionValues)]);
}
