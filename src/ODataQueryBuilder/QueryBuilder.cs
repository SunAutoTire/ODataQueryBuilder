namespace SunAuto.OData;

public class QueryBuilder(params string?[] routeSegments)
{
    public string Route = string.Join('/', routeSegments.Where(rs => !string.IsNullOrWhiteSpace(rs)));
    
    protected IEnumerable<Option> Options { get; set; } = [];

    internal Option Add(Option option)
    {
        Options = [.. Options, option];

        return option;
    }
}
