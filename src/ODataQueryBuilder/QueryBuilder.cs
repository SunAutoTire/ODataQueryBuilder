using System.Globalization;

namespace SunAuto.OData;

/// <summary>
/// Builds an OData query string with a fluent interface.
/// </summary>
/// <param name="routeSegments">
/// Optional route segments joined with <c>/</c> and prefixed to the query string. When omitted the builder
/// produces the query string alone (e.g. <c>?$select=Id</c>).
/// </param>
/// <remarks>
/// Options are emitted in the order they were added. <c>$select</c>, <c>$expand</c> and <c>$orderby</c>
/// accumulate their values into a single option that moves to the position of its most recent addition;
/// <c>$top</c>, <c>$skip</c>, <c>$count</c> and <c>$search</c> replace any earlier value the same way. Each
/// <see cref="Filter(OptionValue[])" /> call adds its own <c>$filter</c> unless <see cref="And" /> or
/// <see cref="Or" /> was called first, in which case the expression is joined onto the preceding one.
/// </remarks>
public class QueryBuilder(params string?[] routeSegments)
{
    private const string FilterName = "filter";
    private const string SelectName = "select";
    private const string ExpandName = "expand";
    private const string OrderByName = "orderby";
    private const string SearchName = "search";
    private const string TopName = "top";
    private const string SkipName = "skip";
    private const string CountName = "count";

    private readonly List<Option> options = [];

    private string? connector;

    /// <summary>Gets the route the query string is appended to; empty when no segments were supplied.</summary>
    public string Route { get; } = string.Join('/', routeSegments.Where(rs => !string.IsNullOrWhiteSpace(rs)));

    #region $filter

    /// <summary>Adds one or more <c>$filter</c> expressions.</summary>
    /// <param name="optionValues">The filter expressions.</param>
    public QueryBuilder Filter(params OptionValue[] optionValues) => Filter(false, optionValues);

    /// <summary>Adds one or more <c>$filter</c> expressions unless <paramref name="ignore"/> is <see langword="true"/>.</summary>
    /// <param name="ignore">When <see langword="true"/> the expressions are discarded.</param>
    /// <param name="optionValues">The filter expressions.</param>
    public QueryBuilder Filter(bool ignore, params OptionValue[] optionValues)
    {
        if (ignore)
            return this;

        foreach (var optionValue in optionValues)
            AddFilter(optionValue);

        return this;
    }

    /// <summary>Joins the next filter expression onto the preceding one with <c>and</c>.</summary>
    public QueryBuilder And()
    {
        connector = "and";
        return this;
    }

    /// <summary>Joins the next filter expression onto the preceding one with <c>or</c>.</summary>
    public QueryBuilder Or()
    {
        connector = "or";
        return this;
    }

    /// <summary>
    /// Wraps the most recent <c>$filter</c> expression in parentheses so it becomes a single boolean operand
    /// for whatever is joined onto it next.
    /// </summary>
    public QueryBuilder ToBool()
    {
        var current = CurrentFilter();

        if (current is not null)
            current.Value = $"({current.Value})";

        return this;
    }

    private void AddFilter(OptionValue optionValue)
    {
        var expression = optionValue.ToString();

        if (string.IsNullOrWhiteSpace(expression))
            return;

        var current = connector is null ? null : CurrentFilter();

        if (current is null)
            options.Add(new Option(FilterName, optionValue));
        else
            current.Value = $"{current.Value} {connector} {expression}";

        connector = null;
    }

    private OptionValue? CurrentFilter()
        => options.LastOrDefault(o => o.Name == FilterName)?.OptionValues.LastOrDefault();

    #endregion

    #region $select, $expand, $orderby

    /// <summary>Adds one or more properties to <c>$select</c>.</summary>
    /// <param name="optionValues">The properties to select.</param>
    public QueryBuilder Select(params OptionValue[] optionValues) => Select(false, optionValues);

    /// <summary>Adds one or more properties to <c>$select</c> unless <paramref name="ignore"/> is <see langword="true"/>.</summary>
    /// <param name="ignore">When <see langword="true"/> the properties are discarded.</param>
    /// <param name="optionValues">The properties to select.</param>
    public QueryBuilder Select(bool ignore, params OptionValue[] optionValues)
        => ignore ? this : Accumulate(SelectName, optionValues);

    /// <summary>Adds one or more navigation properties to <c>$expand</c>.</summary>
    /// <param name="optionValues">The navigation properties to expand.</param>
    public QueryBuilder Expand(params OptionValue[] optionValues) => Expand(false, optionValues);

    /// <summary>Adds one or more navigation properties to <c>$expand</c> unless <paramref name="ignore"/> is <see langword="true"/>.</summary>
    /// <param name="ignore">When <see langword="true"/> the navigation properties are discarded.</param>
    /// <param name="optionValues">The navigation properties to expand.</param>
    public QueryBuilder Expand(bool ignore, params OptionValue[] optionValues)
        => ignore ? this : Accumulate(ExpandName, optionValues);

    /// <summary>Adds one or more properties to <c>$orderby</c> in ascending order.</summary>
    /// <param name="optionValues">The properties to order by.</param>
    public QueryBuilder OrderBy(params OptionValue[] optionValues)
        => Accumulate(OrderByName, optionValues);

    /// <summary>Adds one or more properties to <c>$orderby</c> in descending order.</summary>
    /// <param name="optionValues">The properties to order by.</param>
    public QueryBuilder OrderByDescending(params OptionValue[] optionValues)
        => Accumulate(OrderByName, Extensions.Descending(optionValues));

    private QueryBuilder Accumulate(string name, IEnumerable<OptionValue> optionValues)
    {
        var option = options.FirstOrDefault(o => o.Name == name);

        if (option is null)
            option = new Option(name);
        else
            options.Remove(option);

        foreach (var optionValue in optionValues)
            option.OptionValues.Add(optionValue);

        options.Add(option);

        return this;
    }

    #endregion

    #region $search, $top, $skip, $count

    /// <summary>Sets <c>$search</c>.</summary>
    /// <param name="expression">The free-text search expression.</param>
    public QueryBuilder Search(string expression) => Replace(SearchName, expression);

    /// <summary>Sets <c>$top</c>.</summary>
    /// <param name="value">The maximum number of items to return.</param>
    public QueryBuilder Top(int value) => Replace(TopName, value.ToString(CultureInfo.InvariantCulture));

    /// <summary>Sets <c>$skip</c>.</summary>
    /// <param name="value">The number of items to skip.</param>
    public QueryBuilder Skip(int value) => Replace(SkipName, value.ToString(CultureInfo.InvariantCulture));

    /// <summary>Sets <c>$count</c>.</summary>
    /// <param name="value">Whether the service should include the total count.</param>
    public QueryBuilder Count(bool value = true) => Replace(CountName, value ? "true" : "false");

    private QueryBuilder Replace(string name, string value)
    {
        options.RemoveAll(o => o.Name == name);
        options.Add(new Option(name, value));

        return this;
    }

    #endregion

    /// <summary>Renders the route and query string.</summary>
    /// <returns>The query, or <see cref="Route"/> alone when no options were added.</returns>
    public string Build() => ToString();

    /// <inheritdoc cref="Build" />
    public override string ToString() => options.Count == 0
        ? Route
        : $"{Route}?{string.Join('&', options)}";
}
