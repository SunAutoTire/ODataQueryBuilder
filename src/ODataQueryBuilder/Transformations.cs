namespace SunAuto.OData;

/// <summary>
/// The transformations of the OData Data Aggregation extension, which make up <c>$apply</c>.
/// </summary>
/// <remarks>
/// <para>
/// Static for the same reason <see cref="Functions"/> is: <c>Filter</c>, <c>Compute</c> and <c>Count</c> would
/// otherwise collide with members that already exist on the receivers you would want to call them on.
/// Add <c>using static SunAuto.OData.Transformations;</c> to call them unqualified.
/// </para>
/// <para>
/// Support for <c>$apply</c> is optional — plenty of services do not implement it at all — and the
/// aggregation extension is a separate specification from OData proper.
/// </para>
/// </remarks>
public static class Transformations
{
    #region Aggregate methods

    /// <summary>Builds <c>{expression} with sum as {alias}</c>.</summary>
    /// <param name="expression">The expression to aggregate.</param>
    /// <param name="alias">The name the aggregated value is returned under.</param>
    public static Expression Sum(Expression expression, string alias) => With(expression, "sum", alias);

    /// <summary>Builds <c>{expression} with min as {alias}</c>.</summary>
    /// <param name="expression">The expression to aggregate.</param>
    /// <param name="alias">The name the aggregated value is returned under.</param>
    public static Expression Min(Expression expression, string alias) => With(expression, "min", alias);

    /// <summary>Builds <c>{expression} with max as {alias}</c>.</summary>
    /// <param name="expression">The expression to aggregate.</param>
    /// <param name="alias">The name the aggregated value is returned under.</param>
    public static Expression Max(Expression expression, string alias) => With(expression, "max", alias);

    /// <summary>Builds <c>{expression} with average as {alias}</c>.</summary>
    /// <param name="expression">The expression to aggregate.</param>
    /// <param name="alias">The name the aggregated value is returned under.</param>
    public static Expression Average(Expression expression, string alias) => With(expression, "average", alias);

    /// <summary>Builds <c>{expression} with countdistinct as {alias}</c>.</summary>
    /// <param name="expression">The expression to aggregate.</param>
    /// <param name="alias">The name the aggregated value is returned under.</param>
    public static Expression CountDistinct(Expression expression, string alias) => With(expression, "countdistinct", alias);

    /// <summary>Builds <c>$count as {alias}</c>, the number of items in the group.</summary>
    /// <param name="alias">The name the count is returned under.</param>
    public static Expression Count(string alias) => new($"$count as {alias}");

    #endregion

    #region Transformations

    /// <summary>Builds <c>aggregate({aggregates})</c>.</summary>
    /// <param name="aggregates">The aggregate expressions, built with <see cref="Sum"/> and friends.</param>
    public static Expression Aggregate(params Expression[] aggregates) => Call("aggregate", aggregates);

    /// <summary>Builds <c>groupby(({properties}))</c>.</summary>
    /// <param name="properties">The properties to group by.</param>
    public static Expression GroupBy(params Expression[] properties)
        => new($"groupby(({string.Join(',', properties)}))");

    /// <summary>Builds <c>groupby(({properties}),{transformations})</c>.</summary>
    /// <param name="properties">The properties to group by.</param>
    /// <param name="transformations">Transformations applied within each group, chained with <c>/</c>.</param>
    public static Expression GroupBy(Expression[] properties, params Expression[] transformations)
        => transformations.Length == 0
            ? GroupBy(properties)
            : new($"groupby(({string.Join(',', properties)}),{Chain(transformations)})");

    /// <summary>Builds <c>filter({predicate})</c>, narrowing the set before later transformations run.</summary>
    /// <param name="predicate">The filter expression.</param>
    public static Expression Filter(Expression predicate) => Call("filter", predicate);

    /// <summary>Builds <c>compute({expressions})</c>, each of the form <c>{expression} as {alias}</c>.</summary>
    /// <param name="expressions">The compute expressions.</param>
    public static Expression Compute(params Expression[] expressions) => Call("compute", expressions);

    /// <summary>Builds <c>identity</c>, the transformation that changes nothing.</summary>
    public static Expression Identity() => new("identity");

    /// <summary>Builds <c>topcount({count},{expression})</c>.</summary>
    /// <param name="count">How many items to keep.</param>
    /// <param name="expression">The expression ranked to choose them.</param>
    public static Expression TopCount(int count, Expression expression) => Call("topcount", count, expression);

    /// <inheritdoc cref="TopCount(int, Expression)" />
    public static Expression BottomCount(int count, Expression expression) => Call("bottomcount", count, expression);

    /// <summary>Builds <c>toppercent({percent},{expression})</c>.</summary>
    /// <param name="percent">What share of items to keep, between 0 and 100.</param>
    /// <param name="expression">The expression ranked to choose them.</param>
    public static Expression TopPercent(int percent, Expression expression) => Call("toppercent", percent, expression);

    /// <inheritdoc cref="TopPercent(int, Expression)" />
    public static Expression BottomPercent(int percent, Expression expression) => Call("bottompercent", percent, expression);

    /// <summary>Builds <c>topsum({sum},{expression})</c>, keeping items until their total reaches a value.</summary>
    /// <param name="sum">The total to reach.</param>
    /// <param name="expression">The expression summed and ranked.</param>
    public static Expression TopSum(int sum, Expression expression) => Call("topsum", sum, expression);

    /// <inheritdoc cref="TopSum(int, Expression)" />
    public static Expression BottomSum(int sum, Expression expression) => Call("bottomsum", sum, expression);

    #endregion

    /// <summary>Joins transformations into the <c>/</c>-separated sequence <c>$apply</c> takes.</summary>
    internal static string Chain(IEnumerable<Expression> transformations) => string.Join('/', transformations);

    private static Expression With(Expression expression, string method, string alias)
        => new($"{expression} with {method} as {alias}");

    private static Expression Call(string name, params Expression[] arguments)
        => new($"{name}({string.Join(',', arguments)})");
}
