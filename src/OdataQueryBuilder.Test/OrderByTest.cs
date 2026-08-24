namespace OdataQueryBuilder.Test;

public class OrderByTest
{
    [Fact(DisplayName = "Test1: OrderBy single property")]
    public void Test1()
    {
        var query = new QueryBuilder()
            .OrderBy("Property")
            .Build();

        Assert.Equal("?$orderby=Property", query);
    }
    
    [Fact(DisplayName = "Test2: OrderBy multiple properties")]
    public void Test2()
    {
        var query = new QueryBuilder()
            .OrderBy("Property")
            .OrderBy("AnotherProperty")
            .Build();

        Assert.Equal("?$orderby=Property,AnotherProperty", query);
    }
    
    [Fact(DisplayName = "Test3: OrderBy with filter")]
    public void Test3()
    {
        var query = new QueryBuilder()
            .OrderBy("Property")
            .Filter("Property eq 'Value'")
            .OrderBy("AnotherProperty")
            .Build();

        Assert.Equal("?$filter=Property eq 'Value'&$orderby=Property,AnotherProperty", query);
    }
    
    [Fact(DisplayName = "Test4: OrderByDescending single property")]
    public void Test4()
    {
        var query = new QueryBuilder()
            .OrderByDescending("Property")
            .Build();

        Assert.Equal("?$orderby=Property desc", query);
    }
    
    [Fact(DisplayName = "Test5: OrderBy then OrderByDescending")]
    public void Test5()
    {
        var query = new QueryBuilder()
            .OrderBy("Property")
            .OrderByDescending("AnotherProperty")
            .Build();

        Assert.Equal("?$orderby=Property,AnotherProperty desc", query);
    }
    
    [Fact(DisplayName = "Test6: OrderByDescending with filter")]
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
