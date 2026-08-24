namespace OdataQueryBuilder.Test;

public class SearchTest
{
    [Fact(DisplayName = "Search Single Value")]
    public void Test1()
    {
        var query = new QueryBuilder()
            .Search("value")
            .Build();

        Assert.Equal("?$search=value", query);
    }
}
