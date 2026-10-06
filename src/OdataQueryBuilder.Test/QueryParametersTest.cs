using static SunAuto.OData.Functions;

namespace OdataQueryBuilder.Test;

public class QueryParametersTest
{
    private static Dictionary<string, string?> Pairs(QueryBuilder builder) =>
        builder.ToQueryParameters().ToDictionary(p => p.Key, p => p.Value);

    [Fact(DisplayName = "No Options Gives No Parameters")]
    public void Test1() => Assert.Empty(new QueryBuilder("Products").ToQueryParameters());

    [Fact(DisplayName = "Options Are Name Value Pairs With Prefixes")]
    public void Test2()
    {
        var pairs = Pairs(new QueryBuilder("Products").Select("Name", "Price").Top(5).Count());

        Assert.Equal("Name,Price", pairs["$select"]);
        Assert.Equal("5", pairs["$top"]);
        Assert.Equal("true", pairs["$count"]);
    }

    [Fact(DisplayName = "Route And Keys Are Excluded")]
    public void Test3()
    {
        var pairs = new QueryBuilder("https://example.com", "Products").Key(1).Top(1).ToQueryParameters();

        Assert.Equal([new KeyValuePair<string, string?>("$top", "1")], pairs);
    }

    [Fact(DisplayName = "Order Matches Build")]
    public void Test4()
    {
        var builder = new QueryBuilder()
            .Top(1).Filter("A".Equal(1)).Select("A").Parameter("p1", "x").Count();

        var fromBuild = builder.Build().TrimStart('?').Split('&').Select(p => p.Split('=', 2)[0]);

        Assert.Equal(fromBuild, builder.ToQueryParameters().Select(p => p.Key));
    }

    [Fact(DisplayName = "Literal Escapes Are Reversed")]
    public void Test5()
    {
        const string name = "Milk & Honey 100% + #1";

        var filter = Pairs(new QueryBuilder().Filter("Name".Equal(name)))["$filter"];

        Assert.Equal($"Name eq '{name}'", filter);
        Assert.Contains("%26", new QueryBuilder().Filter("Name".Equal(name)).Build());
    }

    [Fact(DisplayName = "Percent Text In A Literal Survives A Round Trip")]
    public void Test6()
    {
        const string name = "already %26 encoded-looking";

        Assert.Equal($"Name eq '{name}'", Pairs(new QueryBuilder().Filter("Name".Equal(name)))["$filter"]);
    }

    [Fact(DisplayName = "Parameter Aliases Keep Their At Prefix")]
    public void Test7()
    {
        var pairs = Pairs(new QueryBuilder()
            .Filter("Name".Equal(Expression.Parameter("p1")))
            .Parameter("p1", "Milk & Honey"));

        Assert.Equal("Name eq @p1", pairs["$filter"]);
        Assert.Equal("'Milk & Honey'", pairs["@p1"]);
    }

    [Fact(DisplayName = "Nested Options Stay Inside Their Value")]
    public void Test8() =>
        Assert.Equal("Items($select=Name;$top=5)", Pairs(new QueryBuilder().Expand("Items".Select("Name").Top(5)))["$expand"]);

    [Fact(DisplayName = "Values Containing Equals Are Not Split Again")]
    public void Test9() =>
        Assert.Equal("A eq 'x=y'", Pairs(new QueryBuilder().Filter("A".Equal("x=y")))["$filter"]);

    [Fact(DisplayName = "Functions And Logic Render As Build Does")]
    public void Test10() =>
        Assert.Equal("Price gt 10 and contains(tolower(Name),'milk')",
            Pairs(new QueryBuilder().Filter("Price".GreaterThan(10).And(Contains(ToLower("Name"), "milk"))))["$filter"]);
}
