
namespace OdataQueryBuilder.Test;

public class OptionOrderTest
{
    #region Canonical Order

    [Fact(DisplayName = "Options Render In Canonical Order")]
    public void Test1()
    {
        var query = new QueryBuilder()
            .Count()
            .Skip(2)
            .Top(5)
            .OrderBy("Name")
            .Expand("Items")
            .Select("Name")
            .Search("milk")
            .Filter("Price".GreaterThan(10))
            .Compute("Price".Multiply("Qty").As("Total"))
            .Build();

        Assert.Equal(
            "?$compute=Price mul Qty as Total&$filter=Price gt 10&$search=milk&$select=Name&$expand=Items"
                + "&$orderby=Name&$top=5&$skip=2&$count=true",
            query);
    }

    [Fact(DisplayName = "Call Order Does Not Change The Query")]
    public void Test2()
    {
        var forwards = new QueryBuilder("http://example.com")
            .Filter("Price".GreaterThan(10))
            .Select("Name")
            .OrderBy("Name")
            .Top(5)
            .Build();

        var backwards = new QueryBuilder("http://example.com")
            .Top(5)
            .OrderBy("Name")
            .Select("Name")
            .Filter("Price".GreaterThan(10))
            .Build();

        Assert.Equal(forwards, backwards);
    }

    [Fact(DisplayName = "Interleaved Calls Produce One Option Each")]
    public void Test3()
    {
        var query = new QueryBuilder()
            .Select("Name")
            .Top(5)
            .Select("Price")
            .Top(10)
            .Build();

        Assert.Equal("?$select=Name,Price&$top=10", query);
    }

    #endregion

    #region Rendering

    [Fact(DisplayName = "Build And ToString Agree")]
    public void Test4()
    {
        var builder = new QueryBuilder("http://example.com")
            .Filter("Name".Equal("Milk"))
            .Top(5);

        Assert.Equal(builder.Build(), builder.ToString());
    }

    [Fact(DisplayName = "ToUri Encodes The Spaces Build Leaves Readable")]
    public void Test5()
    {
        var builder = new QueryBuilder("http://example.com")
            .Filter("Name".Equal("Milk"));

        Assert.Equal("http://example.com?$filter=Name eq 'Milk'", builder.Build());
        Assert.Equal("http://example.com/?$filter=Name%20eq%20'Milk'", builder.ToUri().AbsoluteUri);
    }

    #endregion
}
