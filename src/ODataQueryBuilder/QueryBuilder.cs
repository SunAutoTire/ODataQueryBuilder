namespace SunAuto.OData;

public class QueryBuilder(params string?[] routeSegments)
{
    public string Route = string.Join('/', routeSegments.Where(rs => !string.IsNullOrWhiteSpace(rs)));

    protected List<Option> Options { get; set; } = [];

    internal void Add(Option? option)
    {
        if (option is not null)
            Options = [.. Options, option];
    }
}
