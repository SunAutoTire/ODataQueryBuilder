namespace OdataQueryBuilder.Test;

public class OrderByTest
{
    [Fact(DisplayName = "OrderBy Single Property")]
    public void Test1()
    {
        var query = new QueryBuilder()
            .OrderBy("Property")
            .Build();

        Assert.Equal("?$orderby=Property", query);
    }
    
    [Fact(DisplayName = "OrderBy Multiple Properties")]
    public void Test2()
    {
        var query = new QueryBuilder()
            .OrderBy("Property")
            .OrderBy("AnotherProperty")
            .Build();

        Assert.Equal("?$orderby=Property,AnotherProperty", query);
    }
    
    [Fact(DisplayName = "OrderBy With Filter")]
    public void Test3()
    {
        var query = new QueryBuilder()
            .OrderBy("Property")
            .Filter("Property eq 'Value'")
            .OrderBy("AnotherProperty")
            .Build();

        Assert.Equal("?$filter=Property eq 'Value'&$orderby=Property,AnotherProperty", query);
    }
    
    [Fact(DisplayName = "OrderByDescending Single Property")]
    public void Test4()
    {
        var query = new QueryBuilder()
            .OrderByDescending("Property")
            .Build();

        Assert.Equal("?$orderby=Property desc", query);
    }
    
    [Fact(DisplayName = "OrderBy Then OrderByDescending")]
    public void Test5()
    {
        var query = new QueryBuilder()
            .OrderBy("Property")
            .OrderByDescending("AnotherProperty")
            .Build();

        Assert.Equal("?$orderby=Property,AnotherProperty desc", query);
    }
    
    [Fact(DisplayName = "OrderByDescending With Filter")]
    public void Test6()
    {
        var query = new QueryBuilder()
            .OrderByDescending("Property")
            .Filter("Property eq 'Value'")
            .OrderByDescending("AnotherProperty")
            .Build();

        Assert.Equal("?$filter=Property eq 'Value'&$orderby=Property desc,AnotherProperty desc", query);
    }
}
