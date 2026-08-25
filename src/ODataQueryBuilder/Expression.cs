using System.Globalization;
using System.Numerics;
using System.Xml;

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
/// The implicit conversions cover the Edm primitive types, each in the form OData's URL syntax expects, which
/// is not always the form <see cref="object.ToString()"/> produces: GUIDs and date/time values are unquoted,
/// durations carry a <c>duration</c> prefix, and an absent value is the <c>null</c> literal rather than an
/// empty string.
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

    /// <summary>Gets the <c>null</c> literal.</summary>
    public static Expression Null => new("null", Precedence.Primary);

    internal Precedence Precedence => precedence;

    /// <summary>Returns the expression text.</summary>
    public override string ToString() => value ?? string.Empty;

    #region Conversions

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

    /// <summary>Treats a GUID as an <c>Edm.Guid</c> literal, which OData writes unquoted.</summary>
    public static implicit operator Expression(Guid value) => From(value);

    /// <summary>Treats an instant as an <c>Edm.DateTimeOffset</c> literal, which OData writes unquoted.</summary>
    public static implicit operator Expression(DateTimeOffset value) => From(value);

    /// <inheritdoc cref="From(DateTime)" />
    public static implicit operator Expression(DateTime value) => From(value);

    /// <summary>Treats a date as an <c>Edm.Date</c> literal.</summary>
    public static implicit operator Expression(DateOnly value) => From(value);

    /// <summary>Treats a time as an <c>Edm.TimeOfDay</c> literal.</summary>
    public static implicit operator Expression(TimeOnly value) => From(value);

    /// <summary>Treats an interval as an <c>Edm.Duration</c> literal.</summary>
    public static implicit operator Expression(TimeSpan value) => From(value);

    /// <summary>Treats bytes as an <c>Edm.Binary</c> literal.</summary>
    public static implicit operator Expression(byte[]? value) => value is null ? Null : From(value);

    /// <summary>Treats an absent value as the <c>null</c> literal.</summary>
    public static implicit operator Expression(int? value) => value is { } present ? From(present) : Null;

    /// <inheritdoc cref="op_Implicit(Nullable{System.Int32})" />
    public static implicit operator Expression(long? value) => value is { } present ? From(present) : Null;

    /// <inheritdoc cref="op_Implicit(Nullable{System.Int32})" />
    public static implicit operator Expression(double? value) => value is { } present ? From(present) : Null;

    /// <inheritdoc cref="op_Implicit(Nullable{System.Int32})" />
    public static implicit operator Expression(float? value) => value is { } present ? From(present) : Null;

    /// <inheritdoc cref="op_Implicit(Nullable{System.Int32})" />
    public static implicit operator Expression(decimal? value) => value is { } present ? From(present) : Null;

    /// <inheritdoc cref="op_Implicit(Nullable{System.Int32})" />
    public static implicit operator Expression(Guid? value) => value is { } present ? From(present) : Null;

    /// <inheritdoc cref="op_Implicit(Nullable{System.Int32})" />
    public static implicit operator Expression(DateTimeOffset? value) => value is { } present ? From(present) : Null;

    /// <inheritdoc cref="op_Implicit(Nullable{System.Int32})" />
    public static implicit operator Expression(DateTime? value) => value is { } present ? From(present) : Null;

    /// <inheritdoc cref="op_Implicit(Nullable{System.Int32})" />
    public static implicit operator Expression(DateOnly? value) => value is { } present ? From(present) : Null;

    /// <inheritdoc cref="op_Implicit(Nullable{System.Int32})" />
    public static implicit operator Expression(TimeOnly? value) => value is { } present ? From(present) : Null;

    /// <inheritdoc cref="op_Implicit(Nullable{System.Int32})" />
    public static implicit operator Expression(TimeSpan? value) => value is { } present ? From(present) : Null;

    #endregion

    #region Literals

    /// <summary>
    /// Builds an OData string literal: quoted, with any embedded <c>'</c> doubled as OData requires, and with
    /// the characters that would otherwise terminate or corrupt the query string percent-encoded. Without the
    /// latter a value containing <c>&amp;</c> would silently truncate the option it appears in. A
    /// <see langword="null"/> is the <c>null</c> literal, which is distinct from the empty string <c>''</c>.
    /// </summary>
    /// <param name="value">The literal text.</param>
    public static Expression Literal(string? value)
        => value is null ? Null : new($"'{Escape(value).Replace("'", "''")}'", Precedence.Primary);

    /// <summary>
    /// Builds a numeric literal, formatted invariantly. Floating point values keep at least one decimal place
    /// so they stay distinguishable from integral ones (<c>2.0</c> rather than <c>2</c>), and the specials are
    /// written the way OData spells them: <c>NaN</c>, <c>INF</c> and <c>-INF</c>.
    /// </summary>
    /// <param name="value">The number.</param>
    public static Expression From<T>(T value) where T : INumber<T>
    {
        if (value is double or float)
        {
            var real = Convert.ToDouble(value, CultureInfo.InvariantCulture);

            if (double.IsNaN(real))
                return new("NaN", Precedence.Primary);

            if (double.IsInfinity(real))
                return new(double.IsPositiveInfinity(real) ? "INF" : "-INF", Precedence.Primary);
        }

        var text = value switch
        {
            double d => d.ToString("R", CultureInfo.InvariantCulture),
            float f => f.ToString("R", CultureInfo.InvariantCulture),
            _ => value.ToString(null, CultureInfo.InvariantCulture) ?? string.Empty,
        };

        var needsPoint = value is double or float or decimal && text.IndexOf('.') < 0 && text.IndexOf('E') < 0;

        return new(needsPoint ? $"{text}.0" : text, Precedence.Primary);
    }

    /// <summary>Builds a boolean literal.</summary>
    /// <param name="value">The boolean.</param>
    public static Expression From(bool value) => new(value ? "true" : "false", Precedence.Primary);

    /// <summary>Builds an <c>Edm.Guid</c> literal, which OData writes unquoted.</summary>
    /// <param name="value">The GUID.</param>
    public static Expression From(Guid value) => new(value.ToString("D", CultureInfo.InvariantCulture), Precedence.Primary);

    /// <summary>Builds an <c>Edm.DateTimeOffset</c> literal, which OData writes unquoted and with an offset.</summary>
    /// <param name="value">The instant.</param>
    public static Expression From(DateTimeOffset value)
    {
        var time = value.ToString(
            value.Ticks % TimeSpan.TicksPerSecond == 0 ? "yyyy-MM-ddTHH:mm:ss" : "yyyy-MM-ddTHH:mm:ss.FFFFFFF",
            CultureInfo.InvariantCulture);

        var offset = value.Offset == TimeSpan.Zero ? "Z" : value.ToString("zzz", CultureInfo.InvariantCulture);

        return new($"{time}{offset}", Precedence.Primary);
    }

    /// <summary>
    /// Builds an <c>Edm.DateTimeOffset</c> literal from a <see cref="DateTime"/>. OData has no offset-less
    /// instant type, so the value is converted the way <see cref="DateTimeOffset(DateTime)"/> converts it:
    /// <see cref="DateTimeKind.Utc"/> becomes <c>Z</c>, and both <see cref="DateTimeKind.Local"/> and
    /// <see cref="DateTimeKind.Unspecified"/> take the machine's current offset. Prefer
    /// <see cref="DateTimeOffset"/> where the offset matters.
    /// </summary>
    /// <param name="value">The instant.</param>
    public static Expression From(DateTime value) => From(new DateTimeOffset(value));

    /// <summary>Builds an <c>Edm.Date</c> literal.</summary>
    /// <param name="value">The date.</param>
    public static Expression From(DateOnly value)
        => new(value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), Precedence.Primary);

    /// <summary>Builds an <c>Edm.TimeOfDay</c> literal.</summary>
    /// <param name="value">The time of day.</param>
    public static Expression From(TimeOnly value)
        => new(
            value.ToString(value.Ticks % TimeSpan.TicksPerSecond == 0 ? "HH:mm:ss" : "HH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture),
            Precedence.Primary);

    /// <summary>Builds an <c>Edm.Duration</c> literal, an ISO 8601 duration behind a <c>duration</c> prefix.</summary>
    /// <param name="value">The interval.</param>
    public static Expression From(TimeSpan value) => new($"duration'{XmlConvert.ToString(value)}'", Precedence.Primary);

    /// <summary>
    /// Builds an <c>Edm.Binary</c> literal. OData spells binary in the URL-safe base64 alphabet, so <c>+</c>
    /// and <c>/</c> become <c>-</c> and <c>_</c>.
    /// </summary>
    /// <param name="value">The bytes.</param>
    public static Expression From(byte[] value)
        => new($"binary'{Convert.ToBase64String(value).Replace('+', '-').Replace('/', '_')}'", Precedence.Primary);

    /// <summary>
    /// Builds an enumeration member literal, written as a namespace-qualified type name followed by the
    /// member in quotes (e.g. <c>Sales.Color'Yellow'</c>).
    /// </summary>
    /// <param name="type">The namespace-qualified enumeration type name.</param>
    /// <param name="member">The member name, or a comma-separated list of members for a flags enumeration.</param>
    public static Expression EnumMember(string type, string member) => new($"{type}'{member}'", Precedence.Primary);

    #endregion

    /// <summary>
    /// Percent-encodes the characters that carry structural meaning in a URL query string. <c>%</c> goes
    /// first so the encoding it introduces is not itself re-encoded.
    /// </summary>
    internal static string Escape(string value) => value
        .Replace("%", "%25")
        .Replace("&", "%26")
        .Replace("#", "%23")
        .Replace("+", "%2B");

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
