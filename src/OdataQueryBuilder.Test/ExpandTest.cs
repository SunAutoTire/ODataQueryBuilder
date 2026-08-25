namespace OdataQueryBuilder.Test;

public class ExpandTest
{
    [Fact(DisplayName = "Expand Single Property")]
    public void Test1()
    {
        var query = new QueryBuilder()
            .Expand("Property")
            .Build();

        Assert.Equal("?$expand=Property", query);
    }
    
    [Fact(DisplayName = "Expand Multiple Properties")]
    public void Test2()
    {
        var query = new QueryBuilder()
            .Expand("Property")
            .Expand("AnotherProperty")
            .Build();

        Assert.Equal("?$expand=Property,AnotherProperty", query);
    }
    
    [Fact(DisplayName = "Expand With Filter")]
    public void Test3()
    {
        var query = new QueryBuilder()
            .Expand("Property")
            .Filter("Property eq 'Value'")
            .Expand("AnotherProperty")
            .Build();

        Assert.Equal("?$filter=Property eq 'Value'&$expand=Property,AnotherProperty", query);
    }

    [Fact(DisplayName = "Expand Nested Property")]
    public void Test4()
    {
        var query = new QueryBuilder()
            .Expand("Property".Expand("SubProperty"))
            .Build();

        Assert.Equal("?$expand=Property($expand=SubProperty)", query);
    }
    
    [Fact(DisplayName = "Expand Multiple Nested Properties")]
    public void Test5()
    {
        var query = new QueryBuilder()
            .Expand("Property".Expand("SubProperty"))
            .Expand("AnotherProperty".Expand("SubAnotherProperty"))
            .Build();

        Assert.Equal("?$expand=Property($expand=SubProperty),AnotherProperty($expand=SubAnotherProperty)", query);
    }
    
    [Fact(DisplayName = "Expand Nested With Filter")]
    public void Test6()
    {
        var query = new QueryBuilder()
            .Expand("Property".Expand("SubProperty"))
            .Filter("Property eq 'Value'")
            .Expand("AnotherProperty".Expand("SubAnotherProperty"))
            .Build();

        Assert.Equal("?$filter=Property eq 'Value'&$expand=Property($expand=SubProperty),AnotherProperty($expand=SubAnotherProperty)", query);
    }

    #region Reuse

    [Fact(DisplayName = "Nested Options Do Not Leak Between Chains")]
    public void Test7()
    {
        var items = "Items".Expand("Parent");

        var filtered = items.Filter("Total".GreaterThan(100));
        var ordered = items.OrderBy("Name");

        Assert.Equal("Items($expand=Parent;$filter=Total gt 100)", filtered.ToString());
        Assert.Equal("Items($expand=Parent;$orderby=Name)", ordered.ToString());
        Assert.Equal("Items($expand=Parent)", items.ToString());
    }

    #endregion
}
