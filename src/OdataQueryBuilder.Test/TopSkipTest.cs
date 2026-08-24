namespace OdataQueryBuilder.Test;

public class TopSkipTest
{
    [Fact(DisplayName = "Test1: Top single value")]
    public void Test1()
    {
        var query = new QueryBuilder()
            .Top(5)
            .Build();

        Assert.Equal("?$top=5", query);
    }

    [Fact(DisplayName = "Test2: Top and Skip")]
    public void Test2()
    {
        var query = new QueryBuilder()
            .Top(5)
            .Skip(2)
            .Build();

        Assert.Equal("?$top=5&$skip=2", query);
    }

    [Fact(DisplayName = "Test3: Skip single value")]
    public void Test3()
    {
        var query = new QueryBuilder()
            .Skip(2)
            .Build();

        Assert.Equal("?$skip=2", query);
    }

    [Fact(DisplayName = "Test4: Count")]
    public void Test4()
    {
        var query = new QueryBuilder()
            .Count()
            .Build();

        Assert.Equal("?$count=true", query);
    }
}
