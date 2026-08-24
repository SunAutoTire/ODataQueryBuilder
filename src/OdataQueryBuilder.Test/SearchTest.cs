namespace OdataQueryBuilder.Test;

public class SearchTest
{
    [Fact(DisplayName = "Test1: Search single value")]
    public void Test1()
    {
        var query = new QueryBuilder()
            .Search("value")
            .Build();

        Assert.Equal("?$search=value", query);
    }
}
