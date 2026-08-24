using System.Globalization;
using System.Numerics;

namespace SunAuto.OData;

/// <summary>
/// How tightly an OData operator binds, lowest first. Used to parenthesise an operand only where leaving it
/// bare would change what the expression means.
/// </summary>
internal enum Precedence
{
    /// <summary><c>or</c>.</summary>
    Or = 1,

    /// <summary><c>and</c>.</summary>
    And = 2,

    /// <summary><c>eq ne gt ge lt le has in</c>.</summary>
    Comparison = 3,

    /// <summary><c>add sub</c>.</summary>
    Additive = 4,

    /// <summary><c>mul div divby mod</c>.</summary>
    Multiplicative = 5,

    /// <summary><c>not</c>.</summary>
    Unary = 6,

    /// <summary>A literal, a property path, or anything already parenthesised.</summary>
    Primary = 7,
}

/// <summary>
/// A fragment of an OData expression — a property path, a literal, or the result of an operator.
/// </summary>
/// <remarks>
/// <para>
/// This type is what keeps the operators from quoting each other's output. A <see cref="string"/> passed to a
/// comparison operator is an OData string literal and gets quoted; an <see cref="Expression"/> is emitted
/// verbatim. Because every operator returns an <see cref="Expression"/>, composing them does the right thing
/// without any escape hatch: <c>"Total".Equal("Price".Multiply("Qty"))</c> yields
/// <c>Total eq Price mul Qty</c>, not <c>Total eq 'Price mul Qty'</c>.
/// </para>
/// <para>
/// Text converted from a <see cref="string"/> is treated as atomic, since the library does not parse it. If a
/// hand-written fragment contains a loosely binding operator, group it yourself before combining it with
/// something tighter.
/// </para>
/// </remarks>
public readonly struct Expression
{
    private readonly string? value;
    private readonly Precedence precedence;

    /// <summary>Wraps text that is already a valid OData expression, treating it as atomic.</summary>
    /// <param name="value">The expression text, emitted verbatim.</param>
    public Expression(string? value) : this(value, Precedence.Primary)
    {
    }

    internal Expression(string? value, Precedence precedence)
    {
        this.value = value;
        this.precedence = precedence;
    }

    internal Precedence Precedence => precedence;

    /// <summary>Returns the expression text.</summary>
    public override string ToString() => value ?? string.Empty;

    /// <summary>Treats a string as an expression, emitted verbatim.</summary>
    public static implicit operator Expression(string? value) => new(value);

    /// <summary>Treats a number as a numeric literal.</summary>
    public static implicit operator Expression(int value) => From(value);

    /// <inheritdoc cref="op_Implicit(int)" />
    public static implicit operator Expression(long value) => From(value);

    /// <inheritdoc cref="op_Implicit(int)" />
    public static implicit operator Expression(double value) => From(value);

    /// <inheritdoc cref="op_Implicit(int)" />
    public static implicit operator Expression(float value) => From(value);

    /// <inheritdoc cref="op_Implicit(int)" />
    public static implicit operator Expression(decimal value) => From(value);

    /// <summary>Treats a boolean as <c>true</c> or <c>false</c>.</summary>
    public static implicit operator Expression(bool value) => new(value ? "true" : "false", Precedence.Primary);

    /// <summary>
    /// Builds an OData string literal: quoted, with any embedded <c>'</c> doubled as OData requires, and with
    /// the characters that would otherwise terminate or corrupt the query string percent-encoded. Without the
    /// latter a value containing <c>&amp;</c> would silently truncate the option it appears in.
    /// </summary>
    /// <param name="value">The literal text.</param>
    public static Expression Literal(string? value)
        => new($"'{Escape(value ?? string.Empty).Replace("'", "''")}'", Precedence.Primary);

    /// <summary>
    /// Percent-encodes the characters that carry structural meaning in a URL query string. <c>%</c> goes
    /// first so the encoding it introduces is not itself re-encoded.
    /// </summary>
    internal static string Escape(string value) => value
        .Replace("%", "%25")
        .Replace("&", "%26")
        .Replace("#", "%23")
        .Replace("+", "%2B");

    /// <summary>
    /// Builds a numeric literal, formatted invariantly. Floating point values keep at least one decimal place
    /// so they stay distinguishable from integral ones (<c>2.0</c> rather than <c>2</c>).
    /// </summary>
    /// <param name="value">The number.</param>
    public static Expression From<T>(T value) where T : INumber<T>
    {
        var text = value switch
        {
            double d => d.ToString("R", CultureInfo.InvariantCulture),
            float f => f.ToString("R", CultureInfo.InvariantCulture),
            _ => value.ToString(null, CultureInfo.InvariantCulture) ?? string.Empty,
        };

        var needsPoint = value is double or float or decimal && text.IndexOf('.') < 0 && text.IndexOf('E') < 0;

        return new(needsPoint ? $"{text}.0" : text, Precedence.Primary);
    }

    internal static Expression Binary(Expression left, string @operator, Expression right, Precedence precedence)
        => new($"{left.Operand(precedence, false)} {@operator} {right.Operand(precedence, true)}", precedence);

    internal static Expression Unary(string @operator, Expression operand)
        => new($"{@operator} {operand.Operand(Precedence.Unary, false)}", Precedence.Unary);

    internal static Expression Grouped(Expression expression) => new($"({expression})", Precedence.Primary);

    internal static Expression Alias(Expression expression, string alias) => new($"{expression} as {alias}", Precedence.Primary);

    internal static Expression List(IEnumerable<Expression> expressions)
        => new($"({string.Join(',', expressions)})", Precedence.Primary);

    /// <summary>
    /// Renders this expression for use as an operand of an operator binding at <paramref name="context"/>,
    /// parenthesising it where precedence would otherwise regroup it. The right operand of a binary operator
    /// is parenthesised at equal precedence too, since operators at the same level are not interchangeable
    /// (<c>a sub (b sub c)</c> is not <c>a sub b sub c</c>).
    /// </summary>
    private string Operand(Precedence context, bool isRight)
    {
        var regrouped = isRight ? precedence <= context : precedence < context;

        return regrouped ? $"({this})" : ToString();
    }
}
