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
/// <para>
/// Options render in a fixed order rather than the order they were added, so a query is a function of what
/// was asked for and not of the order the calls happened to be made. OData attaches no meaning to option
/// order, but URL strings do — cache keys, logs and assertions all churn if it drifts.
/// </para>
/// <para>
/// Each option appears at most once, as OData requires. <c>$select</c>, <c>$expand</c>, <c>$orderby</c> and
/// <c>$compute</c> accumulate their values; <c>$top</c>, <c>$skip</c>, <c>$count</c> and <c>$search</c>
/// replace any earlier value; and repeated <see cref="Filter(Expression[])" /> calls are joined with
/// <c>and</c>, parenthesised where precedence requires it.
/// </para>
/// </remarks>
public class QueryBuilder(params string?[] routeSegments)
{
    private const string ApplyName = "apply";
    private const string ComputeName = "compute";
    private const string FilterName = "filter";
    private const string SearchName = "search";
    private const string SelectName = "select";
    private const string ExpandName = "expand";
    private const string OrderByName = "orderby";
    private const string TopName = "top";
    private const string SkipName = "skip";
    private const string CountName = "count";

    private static readonly string[] RenderOrder =
        [ApplyName, ComputeName, FilterName, SearchName, SelectName, ExpandName, OrderByName, TopName, SkipName, CountName];

    private readonly List<Option> options = [];
    private readonly List<string> segments = [.. routeSegments.Where(rs => !string.IsNullOrWhiteSpace(rs)).Select(rs => rs!)];

    private Expression? filter;
    private string? apply;

    /// <summary>Gets the route the query string is appended to; empty when no segments were supplied.</summary>
    public string Route => string.Join('/', segments);

    #region Route

    /// <summary>
    /// Appends route segments. Use it for the path-only resources too: <c>Segment("$count")</c>,
    /// <c>Segment("$value")</c> and <c>Segment("$ref")</c>.
    /// </summary>
    /// <param name="additional">The segments to append.</param>
    public QueryBuilder Segment(params string?[] additional)
    {
        segments.AddRange(additional.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s!));

        return this;
    }

    /// <summary>Addresses a single entity by a string key, as in <c>Products('Milk')</c>.</summary>
    /// <param name="value">The key, quoted as a string literal.</param>
    public QueryBuilder Key(string value) => AppendKey(Expression.Literal(value).ToString());

    /// <summary>Addresses a single entity by key, as in <c>Products(1)</c>.</summary>
    /// <param name="value">The key, emitted verbatim.</param>
    public QueryBuilder Key(Expression value) => AppendKey(value.ToString());

    /// <summary>Addresses a single entity by a named string key, as in <c>Products(Name='Milk')</c>.</summary>
    /// <param name="name">The key property name.</param>
    /// <param name="value">The key, quoted as a string literal.</param>
    public QueryBuilder Key(string name, string value) => AppendKey($"{name}={Expression.Literal(value)}");

    /// <summary>Addresses a single entity by a named key, as in <c>Products(Id=1)</c>.</summary>
    /// <param name="name">The key property name.</param>
    /// <param name="value">The key, emitted verbatim.</param>
    public QueryBuilder Key(string name, Expression value) => AppendKey($"{name}={value}");

    /// <summary>
    /// Addresses a single entity by a composite key, as in <c>Products(CategoryId=1,Name='Milk')</c>.
    /// </summary>
    /// <param name="parts">The key parts. Quote string values with <see cref="Expression.Literal" />.</param>
    public QueryBuilder Key(params (string Name, Expression Value)[] parts)
        => AppendKey(string.Join(',', parts.Select(p => $"{p.Name}={p.Value}")));

    private QueryBuilder AppendKey(string key)
    {
        if (segments.Count == 0)
            throw new InvalidOperationException("A key predicate needs an entity set to address; add a route segment first.");

        segments[^1] += $"({key})";

        return this;
    }

    #endregion

    #region $apply

    /// <summary>
    /// Adds one or more <c>$apply</c> transformations, chained onto any already added with <c>/</c>. Build
    /// them with <see cref="Transformations" />. Requires the Data Aggregation extension.
    /// </summary>
    /// <param name="transformations">The transformations.</param>
    public QueryBuilder Apply(params Expression[] transformations)
    {
        foreach (var transformation in transformations)
            AddApply(transformation);

        return this;
    }

    /// <summary>Adds one or more <c>$apply</c> transformations only when <paramref name="condition"/> holds.</summary>
    /// <param name="condition">When <see langword="false"/> the transformations are discarded.</param>
    /// <param name="transformations">The transformations.</param>
    public QueryBuilder ApplyIf(bool condition, params Expression[] transformations)
        => condition ? Apply(transformations) : this;

    private void AddApply(Expression transformation)
    {
        var text = transformation.ToString();

        if (string.IsNullOrWhiteSpace(text))
            return;

        apply = apply is null ? text : $"{apply}/{text}";

        Option(ApplyName).Set(apply);
    }

    #endregion

    #region $filter

    /// <summary>Adds one or more <c>$filter</c> expressions, joined with <c>and</c> onto any already added.</summary>
    /// <param name="expressions">The filter expressions.</param>
    public QueryBuilder Filter(params Expression[] expressions)
    {
        foreach (var expression in expressions)
            AddFilter(expression);

        return this;
    }

    /// <summary>Adds one or more <c>$filter</c> expressions only when <paramref name="condition"/> holds.</summary>
    /// <param name="condition">When <see langword="false"/> the expressions are discarded.</param>
    /// <param name="expressions">The filter expressions.</param>
    public QueryBuilder FilterIf(bool condition, params Expression[] expressions)
        => condition ? Filter(expressions) : this;

    private void AddFilter(Expression expression)
    {
        if (string.IsNullOrWhiteSpace(expression.ToString()))
            return;

        filter = filter is { } existing ? existing.And(expression) : expression;

        Option(FilterName).Set(filter.Value.ToString());
    }

    #endregion

    #region $select, $expand, $orderby, $compute

    /// <summary>Adds one or more properties to <c>$select</c>.</summary>
    /// <param name="optionValues">The properties to select.</param>
    public QueryBuilder Select(params OptionValue[] optionValues) => Accumulate(SelectName, optionValues);

    /// <summary>Adds one or more properties to <c>$select</c> only when <paramref name="condition"/> holds.</summary>
    /// <param name="condition">When <see langword="false"/> the properties are discarded.</param>
    /// <param name="optionValues">The properties to select.</param>
    public QueryBuilder SelectIf(bool condition, params OptionValue[] optionValues)
        => condition ? Select(optionValues) : this;

    /// <summary>Adds one or more navigation properties to <c>$expand</c>.</summary>
    /// <param name="optionValues">The navigation properties to expand.</param>
    public QueryBuilder Expand(params OptionValue[] optionValues) => Accumulate(ExpandName, optionValues);

    /// <summary>Adds one or more navigation properties to <c>$expand</c> only when <paramref name="condition"/> holds.</summary>
    /// <param name="condition">When <see langword="false"/> the navigation properties are discarded.</param>
    /// <param name="optionValues">The navigation properties to expand.</param>
    public QueryBuilder ExpandIf(bool condition, params OptionValue[] optionValues)
        => condition ? Expand(optionValues) : this;

    /// <summary>Adds one or more properties to <c>$orderby</c> in ascending order.</summary>
    /// <param name="optionValues">The properties to order by.</param>
    public QueryBuilder OrderBy(params OptionValue[] optionValues) => Accumulate(OrderByName, optionValues);

    /// <summary>Adds one or more properties to <c>$orderby</c> in ascending order only when <paramref name="condition"/> holds.</summary>
    /// <param name="condition">When <see langword="false"/> the properties are discarded.</param>
    /// <param name="optionValues">The properties to order by.</param>
    public QueryBuilder OrderByIf(bool condition, params OptionValue[] optionValues)
        => condition ? OrderBy(optionValues) : this;

    /// <summary>Adds one or more properties to <c>$orderby</c> in descending order.</summary>
    /// <param name="optionValues">The properties to order by.</param>
    public QueryBuilder OrderByDescending(params OptionValue[] optionValues)
        => Accumulate(OrderByName, Extensions.Descending(optionValues));

    /// <summary>Adds one or more properties to <c>$orderby</c> in descending order only when <paramref name="condition"/> holds.</summary>
    /// <param name="condition">When <see langword="false"/> the properties are discarded.</param>
    /// <param name="optionValues">The properties to order by.</param>
    public QueryBuilder OrderByDescendingIf(bool condition, params OptionValue[] optionValues)
        => condition ? OrderByDescending(optionValues) : this;

    /// <summary>
    /// Adds one or more <c>$compute</c> expressions, each of the form <c>{expression} as {alias}</c>. The
    /// aliases they define can then be used from <c>$filter</c>, <c>$orderby</c> and <c>$select</c>.
    /// Requires OData 4.01.
    /// </summary>
    /// <param name="expressions">The compute expressions.</param>
    public QueryBuilder Compute(params Expression[] expressions)
        => Accumulate(ComputeName, [.. expressions.Select(e => (OptionValue)e)]);

    /// <summary>Adds one or more <c>$compute</c> expressions only when <paramref name="condition"/> holds.</summary>
    /// <param name="condition">When <see langword="false"/> the expressions are discarded.</param>
    /// <param name="expressions">The compute expressions.</param>
    public QueryBuilder ComputeIf(bool condition, params Expression[] expressions)
        => condition ? Compute(expressions) : this;

    private QueryBuilder Accumulate(string name, OptionValue[] optionValues)
    {
        var option = Option(name);

        foreach (var optionValue in optionValues)
            option.Add(optionValue);

        return this;
    }

    #endregion

    #region $search, $top, $skip, $count

    /// <summary>Sets <c>$search</c>.</summary>
    /// <param name="expression">The free-text search expression.</param>
    public QueryBuilder Search(string expression) => Replace(SearchName, Expression.Escape(expression));

    /// <summary>Sets <c>$search</c> only when <paramref name="condition"/> holds.</summary>
    /// <param name="condition">When <see langword="false"/> the expression is discarded.</param>
    /// <param name="expression">The free-text search expression.</param>
    public QueryBuilder SearchIf(bool condition, string expression) => condition ? Search(expression) : this;

    /// <summary>Sets <c>$top</c>.</summary>
    /// <param name="value">The maximum number of items to return.</param>
    public QueryBuilder Top(int value) => Replace(TopName, value.ToString(CultureInfo.InvariantCulture));

    /// <summary>Sets <c>$top</c> only when <paramref name="condition"/> holds.</summary>
    /// <param name="condition">When <see langword="false"/> the value is discarded.</param>
    /// <param name="value">The maximum number of items to return.</param>
    public QueryBuilder TopIf(bool condition, int value) => condition ? Top(value) : this;

    /// <summary>Sets <c>$skip</c>.</summary>
    /// <param name="value">The number of items to skip.</param>
    public QueryBuilder Skip(int value) => Replace(SkipName, value.ToString(CultureInfo.InvariantCulture));

    /// <summary>Sets <c>$skip</c> only when <paramref name="condition"/> holds.</summary>
    /// <param name="condition">When <see langword="false"/> the value is discarded.</param>
    /// <param name="value">The number of items to skip.</param>
    public QueryBuilder SkipIf(bool condition, int value) => condition ? Skip(value) : this;

    /// <summary>
    /// Sets <c>$count</c>. There is no <c>CountIf</c> because <paramref name="value"/> already carries the
    /// condition: <c>Count(false)</c> asks the service not to count, which is the service's default anyway.
    /// </summary>
    /// <param name="value">Whether the service should include the total count.</param>
    public QueryBuilder Count(bool value = true) => Replace(CountName, value ? "true" : "false");

    private QueryBuilder Replace(string name, string value)
    {
        Option(name).Set(value);

        return this;
    }

    #endregion

    /// <summary>Renders the route and query string.</summary>
    /// <returns>The query, or <see cref="Route"/> alone when no options were added.</returns>
    public string Build() => options.Count == 0
        ? Route
        : $"{Route}?{string.Join('&', options.OrderBy(RenderIndex))}";

    /// <summary>
    /// Renders the query as a <see cref="Uri"/>, which percent-encodes the characters that are merely invalid
    /// in a URL rather than structural — spaces, most obviously. The characters that would change how the
    /// query parses are already encoded by <see cref="Expression.Literal" /> when the value is built.
    /// </summary>
    public Uri ToUri() => new(Build(), UriKind.RelativeOrAbsolute);

    /// <inheritdoc cref="Build" />
    public override string ToString() => Build();

    private Option Option(string name)
    {
        var option = options.FirstOrDefault(o => o.Name == name);

        if (option is null)
            options.Add(option = new Option(name));

        return option;
    }

    private static int RenderIndex(Option option)
    {
        var index = Array.IndexOf(RenderOrder, option.Name);

        return index < 0 ? int.MaxValue : index;
    }
}
