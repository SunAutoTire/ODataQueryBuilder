namespace SunAuto.OData;

public class OptionValue(string value, params Option[] nestedOptions)
{
    public string Value { get; } = value;

    public IEnumerable<Option> NestedOptions { get; } = nestedOptions;

    public static implicit operator OptionValue(string value) => new(value ?? string.Empty);

    public override string ToString() => NestedOptions.Any()
        ? $"{Value}({string.Join(',', NestedOptions.Select(o => o.ToString()))})"
        : Value;
}