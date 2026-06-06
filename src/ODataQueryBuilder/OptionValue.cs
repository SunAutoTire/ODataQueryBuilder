namespace SunAuto.OData;

public class OptionValue
{
    public OptionValue(string value, params Option[] nestedOptions)
    {
        Value = value;
        NestedOptions = nestedOptions;
    }

    public string Value { get; }
    
    public Option[] NestedOptions { get; }

    public override string ToString() => $"{Value}({string.Join(',', NestedOptions.Select(o => o.ToString()))})";
}