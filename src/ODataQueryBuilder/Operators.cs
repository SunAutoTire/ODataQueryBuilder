using System.Globalization;
using System.Numerics;

namespace SunAuto.OData;

/// <summary>
/// Extension methods that build OData expression fragments from the binary and unary operators.
/// </summary>
/// <remarks>
/// String arguments to the comparison operators are treated as OData string literals: they are quoted and
/// any embedded <c>'</c> is escaped. Numeric and boolean arguments are emitted as-is, as are the operands of
/// the logical operators, which take boolean expressions rather than literals.
/// </remarks>
public static class Operators
{
    #region Comparison operators

    /// <summary>Builds <c>{property} eq {value}</c>.</summary>
    public static string Equal(this string property, string value) => Binary(property, "eq", Quote(value));

    /// <inheritdoc cref="Equal(string, string)" />
    public static string Equal<T>(this string property, T value) where T : INumber<T> => Binary(property, "eq", Number(value));

    /// <inheritdoc cref="Equal(string, string)" />
    public static string Equal(this string property, bool value) => Binary(property, "eq", Bool(value));

    /// <summary>Builds <c>{property} ne {value}</c>.</summary>
    public static string NotEqual(this string property, string value) => Binary(property, "ne", Quote(value));

    /// <inheritdoc cref="NotEqual(string, string)" />
    public static string NotEqual<T>(this string property, T value) where T : INumber<T> => Binary(property, "ne", Number(value));

    /// <inheritdoc cref="NotEqual(string, string)" />
    public static string NotEqual(this string property, bool value) => Binary(property, "ne", Bool(value));

    /// <summary>Builds <c>{property} gt {value}</c>.</summary>
    public static string GreaterThan(this string property, string value) => Binary(property, "gt", Quote(value));

    /// <inheritdoc cref="GreaterThan(string, string)" />
    public static string GreaterThan<T>(this string property, T value) where T : INumber<T> => Binary(property, "gt", Number(value));

    /// <summary>Builds <c>{property} ge {value}</c>.</summary>
    public static string GreaterThanOrEqual(this string property, string value) => Binary(property, "ge", Quote(value));

    /// <inheritdoc cref="GreaterThanOrEqual(string, string)" />
    public static string GreaterThanOrEqual<T>(this string property, T value) where T : INumber<T> => Binary(property, "ge", Number(value));

    /// <summary>Builds <c>{property} lt {value}</c>.</summary>
    public static string LessThan(this string property, string value) => Binary(property, "lt", Quote(value));

    /// <inheritdoc cref="LessThan(string, string)" />
    public static string LessThan<T>(this string property, T value) where T : INumber<T> => Binary(property, "lt", Number(value));

    /// <summary>Builds <c>{property} le {value}</c>.</summary>
    public static string LessThanOrEqual(this string property, string value) => Binary(property, "le", Quote(value));

    /// <inheritdoc cref="LessThanOrEqual(string, string)" />
    public static string LessThanOrEqual<T>(this string property, T value) where T : INumber<T> => Binary(property, "le", Number(value));

    /// <summary>
    /// Builds <c>{property} has {flag}</c>. The flag is emitted verbatim because an OData enumeration member
    /// is written as a qualified type name followed by a quoted member (e.g. <c>Sales.Color'Yellow'</c>).
    /// </summary>
    public static string Has(this string property, string flag) => Binary(property, "has", flag);

    /// <summary>Builds <c>{property} in ({values})</c>. Requires OData 4.01.</summary>
    public static string In(this string property, params string[] values)
        => Binary(property, "in", $"({string.Join(',', values.Select(Quote))})");

    /// <inheritdoc cref="In(string, string[])" />
    public static string In<T>(this string property, params T[] values) where T : INumber<T>
        => Binary(property, "in", $"({string.Join(',', values.Select(Number))})");

    #endregion

    #region Logical operators

    /// <summary>Builds <c>{left} and {right}</c> from two boolean expressions.</summary>
    public static string And(this string left, string right) => Binary(left, "and", right);

    /// <summary>Builds <c>{left} or {right}</c> from two boolean expressions.</summary>
    public static string Or(this string left, string right) => Binary(left, "or", right);

    /// <summary>Builds <c>not {expression}</c>.</summary>
    public static string Not(this string expression) => $"not {expression}";

    #endregion

    #region Arithmetic operators

    /// <summary>Builds <c>{left} add {right}</c>.</summary>
    public static string Add<TLeft, TRight>(this TLeft left, TRight right)
        where TLeft : INumber<TLeft> where TRight : INumber<TRight> => Arithmetic(left, "add", right);

    /// <summary>Builds <c>{left} sub {right}</c>.</summary>
    public static string Subtract<TLeft, TRight>(this TLeft left, TRight right)
        where TLeft : INumber<TLeft> where TRight : INumber<TRight> => Arithmetic(left, "sub", right);

    /// <summary>Builds <c>{left} mul {right}</c>.</summary>
    public static string Multiply<TLeft, TRight>(this TLeft left, TRight right)
        where TLeft : INumber<TLeft> where TRight : INumber<TRight> => Arithmetic(left, "mul", right);

    /// <summary>Builds <c>{left} div {right}</c>, which is integer division when both operands are integral.</summary>
    public static string Divide<TLeft, TRight>(this TLeft left, TRight right)
        where TLeft : INumber<TLeft> where TRight : INumber<TRight> => Arithmetic(left, "div", right);

    /// <summary>Builds <c>{left} divby {right}</c>, which is always fractional division. Requires OData 4.01.</summary>
    public static string DivideBy<TLeft, TRight>(this TLeft left, TRight right)
        where TLeft : INumber<TLeft> where TRight : INumber<TRight> => Arithmetic(left, "divby", right);

    /// <summary>Builds <c>{left} mod {right}</c>.</summary>
    public static string Modulo<TLeft, TRight>(this TLeft left, TRight right)
        where TLeft : INumber<TLeft> where TRight : INumber<TRight> => Arithmetic(left, "mod", right);

    #endregion

    #region Grouping operator

    /// <summary>Wraps an expression in parentheses.</summary>
    public static string Parenthesis(this string expression) => $"({expression})";

    /// <inheritdoc cref="Parenthesis(string)" />
    public static string Parenthesis<T>(this T value) where T : INumber<T> => $"({Number(value)})";

    #endregion

    private static string Binary(string left, string @operator, string right) => $"{left} {@operator} {right}";

    private static string Arithmetic<TLeft, TRight>(TLeft left, string @operator, TRight right)
        where TLeft : INumber<TLeft> where TRight : INumber<TRight>
        => Binary(Number(left), @operator, Number(right));

    private static string Quote(string value) => $"'{value.Replace("'", "''")}'";

    private static string Bool(bool value) => value ? "true" : "false";

    /// <summary>
    /// Formats a number invariantly, keeping floating point values distinguishable from integral ones by
    /// giving them at least one decimal place (<c>2.0</c> rather than <c>2</c>).
    /// </summary>
    private static string Number<T>(T value) where T : INumber<T>
    {
        var text = value switch
        {
            double d => d.ToString("R", CultureInfo.InvariantCulture),
            float f => f.ToString("R", CultureInfo.InvariantCulture),
            _ => value.ToString(null, CultureInfo.InvariantCulture) ?? string.Empty,
        };

        return value is double or float or decimal && text.IndexOf('.') < 0 && text.IndexOf('E') < 0
            ? $"{text}.0"
            : text;
    }
}
