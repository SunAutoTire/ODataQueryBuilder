namespace OdataQueryBuilder.Test;

public class ExpandTest
{
    [Fact(DisplayName = "Test1: Expand single property")]
    public void Test1()
    {
        var query = new QueryBuilder()
            .Expand("Property")
            .Build();

        Assert.Equal("?$expand=Property", query);
    }
    
    [Fact(DisplayName = "Test2: Expand multiple properties")]
    public void Test2()
    {
        var query = new QueryBuilder()
            .Expand("Property")
            .Expand("AnotherProperty")
            .Build();

        Assert.Equal("?$expand=Property,AnotherProperty", query);
    }
    
    [Fact(DisplayName = "Test3: Expand with filter")]
    public void Test3()
    {
        var query = new QueryBuilder()
            .Expand("Property")
            .Filter("Property eq 'Value'")
            .Expand("AnotherProperty")
            .Build();

        Assert.Equal("?$filter=Property eq 'Value'&$expand=Property,AnotherProperty", query);
    }

    [Fact(DisplayName = "Test1: Expand single property")]
    public void Test1()
    {
        var query = new QueryBuilder()
            .Expand("Property".Expand("SubProperty"))
            .Build();

        Assert.Equal("?$expand=Property($expand=SubProperty)", query);
    }
    
    [Fact(DisplayName = "Test2: Expand multiple properties")]
    public void Test2()
    {
        var query = new QueryBuilder()
            .Expand("Property".Expand("SubProperty"))
            .Expand("AnotherProperty".Expand("SubAnotherProperty"))
            .Build();

        Assert.Equal("?$expand=Property($expand=SubProperty),AnotherProperty($expand=SubAnotherProperty)", query);
    }
    
    [Fact(DisplayName = "Test3: Expand with filter")]
    public void Test3()
    {
        var query = new QueryBuilder()
            .Expand("Property".Expand("SubProperty"))
            .Filter("Property eq 'Value'")
            .Expand("AnotherProperty".Expand("SubAnotherProperty"))
            .Build();

        Assert.Equal("?$filter=Property eq 'Value'&$expand=Property($expand=SubProperty),AnotherProperty($expand=SubAnotherProperty)", query);
    }
}
