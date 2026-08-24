using System.Numerics;

namespace SunAuto.OData;

/// <summary>
/// The OData binary and unary operators, as extension methods that compose into an <see cref="Expression"/>.
/// </summary>
/// <remarks>
/// <para>
/// Every operator returns an <see cref="Expression"/> and accepts one, so operators nest without any of them
/// mistaking another's output for a string literal. Parentheses are added automatically wherever precedence
/// would otherwise regroup an operand, so <c>"a".Or("b").And("c")</c> yields <c>(a or b) and c</c>. Use
/// <see cref="Group(Expression)"/> to force parentheses the output would not otherwise need.
/// </para>
/// <para>
/// A <see cref="string"/> argument to a comparison operator is an OData string literal and is quoted;
/// everywhere else a string is an expression fragment and is emitted verbatim, because no other operator
/// accepts a string literal as an operand.
/// </para>
/// </remarks>
public static class Operators
{
    #region Comparison operators

    /// <summary>Builds <c>{left} eq {right}</c>, quoting <paramref name="value"/> as a string literal.</summary>
    public static Expression Equal(this Expression left, string value) => Compare(left, "eq", Expression.Literal(value));

    /// <summary>Builds <c>{left} eq {right}</c>, emitting <paramref name="value"/> verbatim.</summary>
    public static Expression Equal(this Expression left, Expression value) => Compare(left, "eq", value);

    /// <inheritdoc cref="Equal(Expression, string)" />
    public static Expression Equal(this string left, string value) => Compare(left, "eq", Expression.Literal(value));

    /// <inheritdoc cref="Equal(Expression, Expression)" />
    public static Expression Equal(this string left, Expression value) => Compare(left, "eq", value);

    /// <summary>Builds <c>{left} ne {right}</c>, quoting <paramref name="value"/> as a string literal.</summary>
    public static Expression NotEqual(this Expression left, string value) => Compare(left, "ne", Expression.Literal(value));

    /// <summary>Builds <c>{left} ne {right}</c>, emitting <paramref name="value"/> verbatim.</summary>
    public static Expression NotEqual(this Expression left, Expression value) => Compare(left, "ne", value);

    /// <inheritdoc cref="NotEqual(Expression, string)" />
    public static Expression NotEqual(this string left, string value) => Compare(left, "ne", Expression.Literal(value));

    /// <inheritdoc cref="NotEqual(Expression, Expression)" />
    public static Expression NotEqual(this string left, Expression value) => Compare(left, "ne", value);

    /// <summary>Builds <c>{left} gt {right}</c>, quoting <paramref name="value"/> as a string literal.</summary>
    public static Expression GreaterThan(this Expression left, string value) => Compare(left, "gt", Expression.Literal(value));

    /// <summary>Builds <c>{left} gt {right}</c>, emitting <paramref name="value"/> verbatim.</summary>
    public static Expression GreaterThan(this Expression left, Expression value) => Compare(left, "gt", value);

    /// <inheritdoc cref="GreaterThan(Expression, string)" />
    public static Expression GreaterThan(this string left, string value) => Compare(left, "gt", Expression.Literal(value));

    /// <inheritdoc cref="GreaterThan(Expression, Expression)" />
    public static Expression GreaterThan(this string left, Expression value) => Compare(left, "gt", value);

    /// <summary>Builds <c>{left} ge {right}</c>, quoting <paramref name="value"/> as a string literal.</summary>
    public static Expression GreaterThanOrEqual(this Expression left, string value) => Compare(left, "ge", Expression.Literal(value));

    /// <summary>Builds <c>{left} ge {right}</c>, emitting <paramref name="value"/> verbatim.</summary>
    public static Expression GreaterThanOrEqual(this Expression left, Expression value) => Compare(left, "ge", value);

    /// <inheritdoc cref="GreaterThanOrEqual(Expression, string)" />
    public static Expression GreaterThanOrEqual(this string left, string value) => Compare(left, "ge", Expression.Literal(value));

    /// <inheritdoc cref="GreaterThanOrEqual(Expression, Expression)" />
    public static Expression GreaterThanOrEqual(this string left, Expression value) => Compare(left, "ge", value);

    /// <summary>Builds <c>{left} lt {right}</c>, quoting <paramref name="value"/> as a string literal.</summary>
    public static Expression LessThan(this Expression left, string value) => Compare(left, "lt", Expression.Literal(value));

    /// <summary>Builds <c>{left} lt {right}</c>, emitting <paramref name="value"/> verbatim.</summary>
    public static Expression LessThan(this Expression left, Expression value) => Compare(left, "lt", value);

    /// <inheritdoc cref="LessThan(Expression, string)" />
    public static Expression LessThan(this string left, string value) => Compare(left, "lt", Expression.Literal(value));

    /// <inheritdoc cref="LessThan(Expression, Expression)" />
    public static Expression LessThan(this string left, Expression value) => Compare(left, "lt", value);

    /// <summary>Builds <c>{left} le {right}</c>, quoting <paramref name="value"/> as a string literal.</summary>
    public static Expression LessThanOrEqual(this Expression left, string value) => Compare(left, "le", Expression.Literal(value));

    /// <summary>Builds <c>{left} le {right}</c>, emitting <paramref name="value"/> verbatim.</summary>
    public static Expression LessThanOrEqual(this Expression left, Expression value) => Compare(left, "le", value);

    /// <inheritdoc cref="LessThanOrEqual(Expression, string)" />
    public static Expression LessThanOrEqual(this string left, string value) => Compare(left, "le", Expression.Literal(value));

    /// <inheritdoc cref="LessThanOrEqual(Expression, Expression)" />
    public static Expression LessThanOrEqual(this string left, Expression value) => Compare(left, "le", value);

    /// <summary>
    /// Builds <c>{property} has {flag}</c>. The flag is emitted verbatim because an OData enumeration member
    /// is written as a qualified type name followed by a quoted member (e.g. <c>Sales.Color'Yellow'</c>).
    /// </summary>
    public static Expression Has(this Expression property, Expression flag) => Compare(property, "has", flag);

    /// <inheritdoc cref="Has(Expression, Expression)" />
    public static Expression Has(this string property, Expression flag) => Compare(property, "has", flag);

    /// <summary>Builds <c>{property} in ({values})</c>, quoting each value as a string literal. Requires OData 4.01.</summary>
    public static Expression In(this Expression property, params string[] values)
        => Compare(property, "in", Expression.List(values.Select(Expression.Literal)));

    /// <summary>Builds <c>{property} in ({values})</c>, emitting each value verbatim. Requires OData 4.01.</summary>
    public static Expression In(this Expression property, params Expression[] values)
        => Compare(property, "in", Expression.List(values));

    /// <inheritdoc cref="In(Expression, string[])" />
    public static Expression In(this string property, params string[] values)
        => Compare(property, "in", Expression.List(values.Select(Expression.Literal)));

    /// <inheritdoc cref="In(Expression, Expression[])" />
    public static Expression In(this string property, params Expression[] values)
        => Compare(property, "in", Expression.List(values));

    #endregion

    #region Logical operators

    /// <summary>Builds <c>{left} and {right}</c> from two boolean expressions.</summary>
    public static Expression And(this Expression left, Expression right)
        => Expression.Binary(left, "and", right, Precedence.And);

    /// <inheritdoc cref="And(Expression, Expression)" />
    public static Expression And(this string left, Expression right)
        => Expression.Binary(left, "and", right, Precedence.And);

    /// <summary>Builds <c>{left} or {right}</c> from two boolean expressions.</summary>
    public static Expression Or(this Expression left, Expression right)
        => Expression.Binary(left, "or", right, Precedence.Or);

    /// <inheritdoc cref="Or(Expression, Expression)" />
    public static Expression Or(this string left, Expression right)
        => Expression.Binary(left, "or", right, Precedence.Or);

    /// <summary>Builds <c>not {expression}</c>.</summary>
    public static Expression Not(this Expression expression) => Expression.Unary("not", expression);

    /// <inheritdoc cref="Not(Expression)" />
    public static Expression Not(this string expression) => Expression.Unary("not", expression);

    #endregion

    #region Arithmetic operators

    /// <summary>Builds <c>{left} add {right}</c>.</summary>
    public static Expression Add(this Expression left, Expression right) => Arithmetic(left, "add", right);

    /// <inheritdoc cref="Add(Expression, Expression)" />
    public static Expression Add(this string left, Expression right) => Arithmetic(left, "add", right);

    /// <inheritdoc cref="Add(Expression, Expression)" />
    public static Expression Add<TLeft>(this TLeft left, Expression right) where TLeft : INumber<TLeft>
        => Arithmetic(Expression.From(left), "add", right);

    /// <summary>Builds <c>{left} sub {right}</c>.</summary>
    public static Expression Subtract(this Expression left, Expression right) => Arithmetic(left, "sub", right);

    /// <inheritdoc cref="Subtract(Expression, Expression)" />
    public static Expression Subtract(this string left, Expression right) => Arithmetic(left, "sub", right);

    /// <inheritdoc cref="Subtract(Expression, Expression)" />
    public static Expression Subtract<TLeft>(this TLeft left, Expression right) where TLeft : INumber<TLeft>
        => Arithmetic(Expression.From(left), "sub", right);

    /// <summary>Builds <c>{left} mul {right}</c>.</summary>
    public static Expression Multiply(this Expression left, Expression right) => Arithmetic(left, "mul", right);

    /// <inheritdoc cref="Multiply(Expression, Expression)" />
    public static Expression Multiply(this string left, Expression right) => Arithmetic(left, "mul", right);

    /// <inheritdoc cref="Multiply(Expression, Expression)" />
    public static Expression Multiply<TLeft>(this TLeft left, Expression right) where TLeft : INumber<TLeft>
        => Arithmetic(Expression.From(left), "mul", right);

    /// <summary>Builds <c>{left} div {right}</c>, which is integer division when both operands are integral.</summary>
    public static Expression Divide(this Expression left, Expression right) => Arithmetic(left, "div", right);

    /// <inheritdoc cref="Divide(Expression, Expression)" />
    public static Expression Divide(this string left, Expression right) => Arithmetic(left, "div", right);

    /// <inheritdoc cref="Divide(Expression, Expression)" />
    public static Expression Divide<TLeft>(this TLeft left, Expression right) where TLeft : INumber<TLeft>
        => Arithmetic(Expression.From(left), "div", right);

    /// <summary>Builds <c>{left} divby {right}</c>, which is always fractional division. Requires OData 4.01.</summary>
    public static Expression DivideBy(this Expression left, Expression right) => Arithmetic(left, "divby", right);

    /// <inheritdoc cref="DivideBy(Expression, Expression)" />
    public static Expression DivideBy(this string left, Expression right) => Arithmetic(left, "divby", right);

    /// <inheritdoc cref="DivideBy(Expression, Expression)" />
    public static Expression DivideBy<TLeft>(this TLeft left, Expression right) where TLeft : INumber<TLeft>
        => Arithmetic(Expression.From(left), "divby", right);

    /// <summary>Builds <c>{left} mod {right}</c>.</summary>
    public static Expression Modulo(this Expression left, Expression right) => Arithmetic(left, "mod", right);

    /// <inheritdoc cref="Modulo(Expression, Expression)" />
    public static Expression Modulo(this string left, Expression right) => Arithmetic(left, "mod", right);

    /// <inheritdoc cref="Modulo(Expression, Expression)" />
    public static Expression Modulo<TLeft>(this TLeft left, Expression right) where TLeft : INumber<TLeft>
        => Arithmetic(Expression.From(left), "mod", right);

    #endregion

    #region Grouping operator

    /// <summary>Wraps an expression in parentheses that the output would not otherwise need.</summary>
    public static Expression Group(this Expression expression) => Expression.Grouped(expression);

    /// <inheritdoc cref="Group(Expression)" />
    public static Expression Group(this string expression) => Expression.Grouped(expression);

    /// <inheritdoc cref="Group(Expression)" />
    public static Expression Group<T>(this T value) where T : INumber<T> => Expression.Grouped(Expression.From(value));

    #endregion

    #region Alias operator

    /// <summary>
    /// Builds <c>{expression} as {alias}</c>, the form each <c>$compute</c> item takes. The alias is emitted
    /// verbatim because it declares a property name rather than supplying a value.
    /// </summary>
    public static Expression As(this Expression expression, string alias) => Expression.Alias(expression, alias);

    /// <inheritdoc cref="As(Expression, string)" />
    public static Expression As(this string expression, string alias) => Expression.Alias(expression, alias);

    #endregion

    private static Expression Compare(Expression left, string @operator, Expression right)
        => Expression.Binary(left, @operator, right, Precedence.Comparison);

    private static Expression Arithmetic(Expression left, string @operator, Expression right)
        => Expression.Binary(left, @operator, right, @operator is "add" or "sub" ? Precedence.Additive : Precedence.Multiplicative);
}
