namespace SunAuto.OData;

/// <summary>
/// The OData canonical functions.
/// </summary>
/// <remarks>
/// <para>
/// These are static rather than extension methods, unlike the operators. Most of their names — <c>Contains</c>,
/// <c>StartsWith</c>, <c>ToLower</c>, <c>Substring</c>, <c>IndexOf</c>, <c>Trim</c> — are already instance
/// methods on <see cref="string"/>, and an instance method always wins over an extension method. An extension
/// named <c>Contains</c> would be silently shadowed on exactly the receiver you would want it on, so
/// <c>"Name".Contains("Milk")</c> would quietly evaluate to a <see cref="bool"/> instead of building a filter.
/// Making them static removes the shadowing entirely and reads close to the wire syntax.
/// </para>
/// <para>
/// Add <c>using static SunAuto.OData.Functions;</c> to call them unqualified:
/// <c>Filter(Contains("Name", "Milk"))</c>.
/// </para>
/// <para>
/// Where a function takes a value to compare against, a <see cref="string"/> argument is an OData string
/// literal and is quoted, and an <see cref="Expression"/> is emitted verbatim — the same rule the comparison
/// operators follow.
/// </para>
/// </remarks>
public static class Functions
{
    #region String and collection functions

    /// <summary>Builds <c>contains({subject},{value})</c>. <paramref name="value"/> is quoted as a string literal.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    /// <param name="value">The literal to compare against.</param>
    public static Expression Contains(Expression subject, string value) => Call("contains", subject, Expression.Literal(value));

    /// <summary>Builds <c>contains({subject},{value})</c>. <paramref name="value"/> is emitted verbatim.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    /// <param name="value">The expression to compare against.</param>
    public static Expression Contains(Expression subject, Expression value) => Call("contains", subject, value);

    /// <summary>Builds <c>startswith({subject},{value})</c>. <paramref name="value"/> is quoted as a string literal.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    /// <param name="value">The literal to compare against.</param>
    public static Expression StartsWith(Expression subject, string value) => Call("startswith", subject, Expression.Literal(value));

    /// <summary>Builds <c>startswith({subject},{value})</c>. <paramref name="value"/> is emitted verbatim.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    /// <param name="value">The expression to compare against.</param>
    public static Expression StartsWith(Expression subject, Expression value) => Call("startswith", subject, value);

    /// <summary>Builds <c>endswith({subject},{value})</c>. <paramref name="value"/> is quoted as a string literal.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    /// <param name="value">The literal to compare against.</param>
    public static Expression EndsWith(Expression subject, string value) => Call("endswith", subject, Expression.Literal(value));

    /// <summary>Builds <c>endswith({subject},{value})</c>. <paramref name="value"/> is emitted verbatim.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    /// <param name="value">The expression to compare against.</param>
    public static Expression EndsWith(Expression subject, Expression value) => Call("endswith", subject, value);

    /// <summary>Builds <c>indexof({subject},{value})</c>, the zero-based position of a substring. <paramref name="value"/> is quoted as a string literal.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    /// <param name="value">The literal to compare against.</param>
    public static Expression IndexOf(Expression subject, string value) => Call("indexof", subject, Expression.Literal(value));

    /// <summary>Builds <c>indexof({subject},{value})</c>, the zero-based position of a substring. <paramref name="value"/> is emitted verbatim.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    /// <param name="value">The expression to compare against.</param>
    public static Expression IndexOf(Expression subject, Expression value) => Call("indexof", subject, value);

    /// <summary>Builds <c>concat({subject},{value})</c>, joining two strings or two collections. <paramref name="value"/> is quoted as a string literal.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    /// <param name="value">The literal to compare against.</param>
    public static Expression Concat(Expression subject, string value) => Call("concat", subject, Expression.Literal(value));

    /// <summary>Builds <c>concat({subject},{value})</c>, joining two strings or two collections. <paramref name="value"/> is emitted verbatim.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    /// <param name="value">The expression to compare against.</param>
    public static Expression Concat(Expression subject, Expression value) => Call("concat", subject, value);

    /// <summary>Builds <c>matchesPattern({subject},{value})</c>, an ECMAScript regular expression match. Requires OData 4.01. <paramref name="value"/> is quoted as a string literal.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    /// <param name="value">The literal to compare against.</param>
    public static Expression MatchesPattern(Expression subject, string value) => Call("matchesPattern", subject, Expression.Literal(value));

    /// <summary>Builds <c>matchesPattern({subject},{value})</c>, an ECMAScript regular expression match. Requires OData 4.01. <paramref name="value"/> is emitted verbatim.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    /// <param name="value">The expression to compare against.</param>
    public static Expression MatchesPattern(Expression subject, Expression value) => Call("matchesPattern", subject, value);

    /// <summary>Builds <c>substring({subject},{start})</c>.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    /// <param name="start">The zero-based index to start at.</param>
    public static Expression Substring(Expression subject, Expression start) => Call("substring", subject, start);

    /// <summary>Builds <c>substring({subject},{start},{length})</c>.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    /// <param name="start">The zero-based index to start at.</param>
    /// <param name="length">The number of characters to take.</param>
    public static Expression Substring(Expression subject, Expression start, Expression length)
        => Call("substring", subject, start, length);

    /// <summary>Builds <c>hassubset({subject},{subset})</c>. Requires OData 4.01.</summary>
    /// <param name="subject">The collection to test.</param>
    /// <param name="subset">The collection whose members must all be present.</param>
    public static Expression HasSubset(Expression subject, Expression subset) => Call("hassubset", subject, subset);

    /// <summary>Builds <c>hassubsequence({subject},{subsequence})</c>. Requires OData 4.01.</summary>
    /// <param name="subject">The collection to test.</param>
    /// <param name="subsequence">The collection whose members must appear in order.</param>
    public static Expression HasSubsequence(Expression subject, Expression subsequence)
        => Call("hassubsequence", subject, subsequence);

    #endregion

    #region Single argument functions

    /// <summary>Builds <c>length({subject})</c>, the number of characters in a string.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    public static Expression Length(Expression subject) => Call("length", subject);

    /// <summary>Builds <c>tolower({subject})</c>.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    public static Expression ToLower(Expression subject) => Call("tolower", subject);

    /// <summary>Builds <c>toupper({subject})</c>.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    public static Expression ToUpper(Expression subject) => Call("toupper", subject);

    /// <summary>Builds <c>trim({subject})</c>.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    public static Expression Trim(Expression subject) => Call("trim", subject);

    /// <summary>Builds <c>year({subject})</c>.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    public static Expression Year(Expression subject) => Call("year", subject);

    /// <summary>Builds <c>month({subject})</c>.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    public static Expression Month(Expression subject) => Call("month", subject);

    /// <summary>Builds <c>day({subject})</c>.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    public static Expression Day(Expression subject) => Call("day", subject);

    /// <summary>Builds <c>hour({subject})</c>.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    public static Expression Hour(Expression subject) => Call("hour", subject);

    /// <summary>Builds <c>minute({subject})</c>.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    public static Expression Minute(Expression subject) => Call("minute", subject);

    /// <summary>Builds <c>second({subject})</c>.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    public static Expression Second(Expression subject) => Call("second", subject);

    /// <summary>Builds <c>fractionalseconds({subject})</c>.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    public static Expression FractionalSeconds(Expression subject) => Call("fractionalseconds", subject);

    /// <summary>Builds <c>date({subject})</c>, the <c>Edm.Date</c> part of an instant.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    public static Expression Date(Expression subject) => Call("date", subject);

    /// <summary>Builds <c>time({subject})</c>, the <c>Edm.TimeOfDay</c> part of an instant.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    public static Expression Time(Expression subject) => Call("time", subject);

    /// <summary>Builds <c>totaloffsetminutes({subject})</c>.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    public static Expression TotalOffsetMinutes(Expression subject) => Call("totaloffsetminutes", subject);

    /// <summary>Builds <c>totalseconds({subject})</c>, the length of a duration in seconds.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    public static Expression TotalSeconds(Expression subject) => Call("totalseconds", subject);

    /// <summary>Builds <c>round({subject})</c>.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    public static Expression Round(Expression subject) => Call("round", subject);

    /// <summary>Builds <c>floor({subject})</c>.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    public static Expression Floor(Expression subject) => Call("floor", subject);

    /// <summary>Builds <c>ceiling({subject})</c>.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    public static Expression Ceiling(Expression subject) => Call("ceiling", subject);

    /// <summary>Builds <c>geo.length({subject})</c>, the length of a LineString.</summary>
    /// <param name="subject">The expression the function applies to.</param>
    public static Expression GeoLength(Expression subject) => Call("geo.length", subject);

    #endregion

    #region Nullary functions

    /// <summary>Builds <c>now()</c>, the current instant at the service.</summary>
    public static Expression Now() => Call("now");

    /// <summary>Builds <c>maxdatetime()</c>, the largest possible <c>Edm.DateTimeOffset</c>.</summary>
    public static Expression MaxDateTime() => Call("maxdatetime");

    /// <summary>Builds <c>mindatetime()</c>, the smallest possible <c>Edm.DateTimeOffset</c>.</summary>
    public static Expression MinDateTime() => Call("mindatetime");

    #endregion

    #region Lambda operators

    /// <summary>
    /// Builds <c>{collection}/any()</c>, which holds when the collection has any member at all.
    /// </summary>
    /// <param name="collection">The collection to test.</param>
    public static Expression Any(Expression collection) => new($"{collection}/any()");

    /// <summary>Builds <c>{collection}/any({variable}: {predicate})</c>.</summary>
    /// <param name="collection">The collection to test.</param>
    /// <param name="variable">The range variable name the predicate refers to.</param>
    /// <param name="predicate">The predicate, referring to members through <paramref name="variable"/>.</param>
    public static Expression Any(Expression collection, string variable, Expression predicate)
        => new($"{collection}/any({variable}: {predicate})");

    /// <summary>
    /// Builds <c>{collection}/any({variable}: {predicate})</c>, handing the range variable to
    /// <paramref name="predicate"/> so the name is written once rather than repeated in the body.
    /// </summary>
    /// <param name="collection">The collection to test.</param>
    /// <param name="variable">The range variable name.</param>
    /// <param name="predicate">Builds the predicate from the bound range variable.</param>
    public static Expression Any(Expression collection, string variable, Func<Expression, Expression> predicate)
        => Any(collection, variable, predicate(new Expression(variable)));

    /// <summary>Builds <c>{collection}/all({variable}: {predicate})</c>.</summary>
    /// <param name="collection">The collection to test.</param>
    /// <param name="variable">The range variable name the predicate refers to.</param>
    /// <param name="predicate">The predicate, referring to members through <paramref name="variable"/>.</param>
    public static Expression All(Expression collection, string variable, Expression predicate)
        => new($"{collection}/all({variable}: {predicate})");

    /// <inheritdoc cref="Any(Expression, string, Func{Expression, Expression})" />
    public static Expression All(Expression collection, string variable, Func<Expression, Expression> predicate)
        => All(collection, variable, predicate(new Expression(variable)));

    #endregion

    #region Type functions

    /// <summary>Builds <c>cast({type})</c>, casting the current instance.</summary>
    /// <param name="type">The namespace-qualified type name, emitted verbatim.</param>
    public static Expression Cast(string type) => Call("cast", new Expression(type));

    /// <summary>Builds <c>cast({subject},{type})</c>.</summary>
    /// <param name="subject">The expression to cast.</param>
    /// <param name="type">The namespace-qualified type name, emitted verbatim.</param>
    public static Expression Cast(Expression subject, string type) => Call("cast", subject, new Expression(type));

    /// <summary>Builds <c>isof({type})</c>, testing the current instance.</summary>
    /// <param name="type">The namespace-qualified type name, emitted verbatim.</param>
    public static Expression IsOf(string type) => Call("isof", new Expression(type));

    /// <summary>Builds <c>isof({subject},{type})</c>.</summary>
    /// <param name="subject">The expression to test.</param>
    /// <param name="type">The namespace-qualified type name, emitted verbatim.</param>
    public static Expression IsOf(Expression subject, string type) => Call("isof", subject, new Expression(type));

    #endregion

    #region Geo functions

    /// <summary>Builds <c>geo.distance({from},{to})</c>.</summary>
    /// <param name="from">The first point.</param>
    /// <param name="to">The second point.</param>
    public static Expression GeoDistance(Expression from, Expression to) => Call("geo.distance", from, to);

    /// <summary>Builds <c>geo.intersects({point},{area})</c>.</summary>
    /// <param name="point">The point to test.</param>
    /// <param name="area">The polygon to test against.</param>
    public static Expression GeoIntersects(Expression point, Expression area) => Call("geo.intersects", point, area);

    #endregion

    private static Expression Call(string name, params Expression[] arguments)
        => new($"{name}({string.Join(',', arguments)})");
}
