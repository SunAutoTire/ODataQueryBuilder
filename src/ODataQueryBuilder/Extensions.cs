namespace SunAuto.OData;

public static class Extensions
{
    public static Option Select(this QueryBuilder builder, params OptionValue[] optionValues) =>
        builder.Add(new SelectOption(optionValues));
    public static Option Select(this QueryBuilder builder, OptionValue[] optionValues, Func<QueryBuilder, Option>? nested) {
        builder.Add(new SelectOption(optionValues));
        builder.Add( nested?.Invoke(builder));}
    // public static Option Select(this QueryBuilder builder, params OptionValue[] optionValues) => builder.Add(new SelectOption(optionValues));
    // public static Option Select(this QueryBuilder builder, params OptionValue[] optionValues) => builder.Add(new SelectOption(optionValues));
    // public static Option Select(this QueryBuilder builder, string optionValue, Func<QueryBuilder, Option>? nested = null) => builder.Add(new SelectOption(optionValue, nested?.Invoke(new QueryBuilder())));
    // public static Option Select(this Option builder, params string[] optionValues) => builder.Add(new SelectOption(optionValues));
    // public static Option Select(this Option builder, string optionValue, Func<QueryBuilder, Option>? nested = null) => builder.Add(new SelectOption(optionValue, nested?.Invoke(new QueryBuilder())));

    public static Option Expand(this QueryBuilder builder, params OptionValue[] optionValues) => 
        new ExpandOption( optionValues);
    public static Option OrderBy(this QueryBuilder builder, params OptionValue[] optionValues) => 
        new OrderByOption( optionValues);
    public static Option Expand(this string targetOptionValue, params OptionValue[] optionValues) => new ExpandOption(targetOptionValue, optionValues);

    // public static Option Filter(this QueryBuilder builder, params string[] optionValues) => builder.Add(new FilterOption(optionValues));
    // public static Option Filter(this Option builder, params string[] optionValues) => builder.Add(new FilterOption(optionValues));

    // // public static Option Expand(this QueryBuilder builder, params string[] optionValues, Func<QueryBuilder, Option>? nested = null) => builder.Add(new ExpandOption(optionValues, nested?.Invoke(new QueryBuilder())));
    // // public static Option Expand(this Option builder, params string[] optionValues, Func<QueryBuilder, Option>? nested = null) => builder.Add(new ExpandOption(optionValues, nested?.Invoke(new QueryBuilder())));

    // public static Option Count(this QueryBuilder builder) => builder.Add(new CountOption());
    // public static Option Count(this Option builder) => builder.Add(new CountOption());

    // public static Option OrderBy(this QueryBuilder builder, string propertyName, Func<QueryBuilder, Option>? nested = null) => builder.Add(new OrderByOption(propertyName, nested?.Invoke(new QueryBuilder())));
    // public static Option OrderBy(this Option builder, string propertyName, Func<QueryBuilder, Option>? nested = null) => builder.Add(new OrderByOption(propertyName, nested?.Invoke(new QueryBuilder())));
    // public static Option OrderByDescending(this QueryBuilder builder, string propertyName, Func<QueryBuilder, Option>? nested = null) => builder.Add(new OrderByOption(propertyName, nested?.Invoke(new QueryBuilder()), true));
    // public static Option OrderByDescending(this Option builder, string propertyName, Func<QueryBuilder, Option>? nested = null) => builder.Add(new OrderByOption(propertyName, nested?.Invoke(new QueryBuilder()), true));

    public static string Eq(this string item, string value) => $"{item} eq '{value}'";
    public static string Ne(this string item, string value) => $"{item} ne '{value}'";
    public static string And(this string item, string value) => $"({item} and '{value}')";
    public static string Or(this string item, string value) => $"({item} or '{value}')";
    public static string Gt(this string item, string value) => $"{item} gt '{value}'";
    public static string Ge(this string item, string value) => $"{item} ge '{value}'";
    public static string Lt(this string item, string value) => $"{item} lt '{value}'";
    public static string Le(this string item, string value) => $"{item} le '{value}'";
    public static string Not(this string item) => $"not {item}";

    public static string Eq(this string item, int value) => $"{item} eq {value}";
    public static string Ne(this string item, int value) => $"{item} ne {value}";
    public static string Gt(this string item, int value) => $"{item} gt {value}";
    public static string Ge(this string item, int value) => $"{item} ge {value}";
    public static string Lt(this string item, int value) => $"{item} lt {value}";
    public static string Le(this string item, int value) => $"{item} le {value}";

    public static string Eq(this string item, bool value) => $"{item} eq {value}";
    public static string Ne(this string item, bool value) => $"{item} ne {value}";
}

public class FilterOption(params object[] optionValues) : Option(optionValues)
{
    protected override string Name => "filter";
}

public class ExpandOption(params object[] optionValues) : Option(optionValues)
{
    public ExpandOption(string targetOptionValue, params OptionValue[] optionValues) : this(new OptionValue(targetOptionValue, new SelectOption(optionValues)))
    {
    }

    protected override string Name => "expand";
}

public class SelectOption(params object[] optionValues) : Option(optionValues)
{
    protected override string Name => "select";
}

public class OrderByOption(params object[] optionValues) : Option(optionValues)
{
    protected override string Name => "orderby";
}

public class OrderByDescendingOption(params object[] optionValues) : Option(optionValues)
{
    protected override string Name => "orderby";
    protected override string Suffix => " desc";
}

public class CountOption() : Option()
{
    protected override string Name => "count";
}

public class SkipOption(int value) : Option(value)
{
    protected override string Name => "skip";
}

public class TopOption(int value) : Option(value)
{
    protected override string Name => "top";
}