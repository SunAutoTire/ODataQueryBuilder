
namespace OdataQueryBuilder.Test;

public class FilterTest
{
    #region String Arguments

    [Fact(DisplayName = "Filter Single")]
    public void Test1()
    {
        var query = new QueryBuilder()
            .Filter("Property eq 'Value'")
            .Build();

        Assert.Equal("?$filter=Property eq 'Value'", query);
    }

    [Theory(DisplayName = "Filter Conditional")]
    [InlineData(true, "Property eq 'Value'")]
    [InlineData(false, "Property eq 'Value'")]
    public void Test9(bool include, string filter)
    {
        var query = new QueryBuilder()
            .FilterIf(include, filter)
            .Build();

        if (include)
            Assert.Equal("?$filter=" + filter, query);
        else
            Assert.Equal(string.Empty, query);
    }

    [Fact(DisplayName = "Filter And")]
    public void Test2()
    {
        var query = new QueryBuilder()
            .Filter("Property eq 'Value'".And("Property2 eq 'Value2'"))
            .Build();

        Assert.Equal("?$filter=Property eq 'Value' and Property2 eq 'Value2'", query);
    }

    [Fact(DisplayName = "Filter Or")]
    public void Test3()
    {
        var query = new QueryBuilder()
            .Filter("Property eq 'Value'".Or("Property2 eq 'Value2'"))
            .Build();

        Assert.Equal("?$filter=Property eq 'Value' or Property2 eq 'Value2'", query);
    }

    [Fact(DisplayName = "Filter And Or")]
    public void Test4()
    {
        var query = new QueryBuilder()
            .Filter("Property eq 'Value'"
                .And("Property2 eq 'Value2'")
                .Group()
                .Or("Property3 eq 'Value3'"))
            .Build();

        Assert.Equal("?$filter=(Property eq 'Value' and Property2 eq 'Value2') or Property3 eq 'Value3'", query);
    }

    #endregion

    #region Explicit Arguments

    [Fact(DisplayName = "Filter Single From Operators")]
    public void Test5()
    {
        var query = new QueryBuilder()
            .Filter("Property".Equal("Value"))
            .Build();

        Assert.Equal("?$filter=Property eq 'Value'", query);
    }

    [Fact(DisplayName = "Filter And From Operators")]
    public void Test6()
    {
        var query = new QueryBuilder()
            .Filter("Property".Equal("Value").And("Property2".Equal("Value2")))
            .Build();

        Assert.Equal("?$filter=Property eq 'Value' and Property2 eq 'Value2'", query);
    }

    [Fact(DisplayName = "Filter Or From Operators")]
    public void Test7()
    {
        var query = new QueryBuilder()
            .Filter("Property".Equal("Value").Or("Property2".Equal("Value2")))
            .Build();

        Assert.Equal("?$filter=Property eq 'Value' or Property2 eq 'Value2'", query);
    }

    [Fact(DisplayName = "Filter And Or From Operators")]
    public void Test8()
    {
        var query = new QueryBuilder()
            .Filter("Property".Equal("Value")
                .And("Property2".Equal("Value2"))
                .Group()
                .Or("Property3".Equal("Value3")))
            .Build();

        Assert.Equal("?$filter=(Property eq 'Value' and Property2 eq 'Value2') or Property3 eq 'Value3'", query);
    }

    #endregion

    #region Repeated Filter

    [Fact(DisplayName = "Repeated Filter Joins With And")]
    public void Test10()
    {
        var query = new QueryBuilder()
            .Filter("Property".Equal("Value"))
            .Filter("Property2".Equal("Value2"))
            .Build();

        Assert.Equal("?$filter=Property eq 'Value' and Property2 eq 'Value2'", query);
    }

    [Fact(DisplayName = "Repeated Filter Groups A Disjunction Before Joining")]
    public void Test11()
    {
        var query = new QueryBuilder()
            .Filter("Property".Equal("Value").Or("Property2".Equal("Value2")))
            .Filter("Property3".Equal("Value3"))
            .Build();

        Assert.Equal("?$filter=(Property eq 'Value' or Property2 eq 'Value2') and Property3 eq 'Value3'", query);
    }

    [Fact(DisplayName = "Repeated Filter Stays In One Option")]
    public void Test12()
    {
        var query = new QueryBuilder("http://example.com")
            .Filter("Property".Equal("Value"))
            .Skip(2)
            .Filter("Property2".Equal("Value2"))
            .Build();

        Assert.Equal("http://example.com?$filter=Property eq 'Value' and Property2 eq 'Value2'&$skip=2", query);
    }

    #endregion
}
