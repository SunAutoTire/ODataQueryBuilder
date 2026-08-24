
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

    [Theory(DisplayName = "Filter Single")]
    [InlineData(false, "Property eq 'Value'")]
    [InlineData(true, "Property eq 'Value'")]
    public void Test1(bool ignore, string filter)
    {
        var query = new QueryBuilder()
            .Filter(ignore, filter)
            .Build();

        if (ignore)
            Assert.Equal(string.Empty, query);
        else
            Assert.Equal("?$filter=" + filter, query);
    }

    [Fact(DisplayName = "Filter And")]
    public void Test2()
    {
        var query = new QueryBuilder()
            .Filter("Property eq 'Value'")
            .And()
            .Filter("Property2 eq 'Value2'")
            .Build();

        Assert.Equal("?$filter=Property eq 'Value' and Property2 eq 'Value2'", query);
    }
    [Fact(DisplayName = "Filter Or")]

    public void Test3()
    {
        var query = new QueryBuilder()
            .Filter("Property eq 'Value'")
            .Or()
            .Filter("Property2 eq 'Value2'")
            .Build();

        Assert.Equal("?$filter=Property eq 'Value' or Property2 eq 'Value2'", query);
    }

    [Fact(DisplayName = "Filter And Or")]
    public void Test4()
    {
        var query = new QueryBuilder()
            .Filter("Property eq 'Value'")
            .And()
            .Filter("Property2 eq 'Value2'")
            .ToBool()
            .Or()
            .Filter("Property3 eq 'Value3'")
            .Build();

        Assert.Equal("?$filter=(Property eq 'Value' and Property2 eq 'Value2') or Property3 eq 'Value3'", query);
    }

    #endregion

    #region Explicit Arguments

    [Fact(DisplayName = "Filter Single")]
    public void Test5()
    {
        var query = new QueryBuilder()
            .Filter("Property".Equal("Value"))
            .Build();

        Assert.Equal("?$filter=Property eq 'Value'", query);
    }

    [Fact(DisplayName = "Filter And")]
    public void Test6()
    {
        var query = new QueryBuilder()
            .Filter("Property".Equal("Value"))
            .And()
            .Filter("Property2".Equal("Value2"))
            .Build();

        Assert.Equal("?$filter=Property eq 'Value' and Property2 eq 'Value2'", query);
    }
    [Fact(DisplayName = "Filter Or")]

    public void Test7()
    {
        var query = new QueryBuilder()
            .Filter("Property".Equal("Value"))
            .Or()
            .Filter("Property2".Equal("Value2"))
            .Build();

        Assert.Equal("?$filter=Property eq 'Value' or Property2 eq 'Value2'", query);
    }

    [Fact(DisplayName = "Filter And Or")]
    public void Test8()
    {
        var query = new QueryBuilder()
            .Filter("Property".Equal("Value"))
            .And()
            .Filter("Property2".Equal("Value2"))
            .ToBool()
            .Or()
            .Filter("Property3".Equal("Value3"))
            .Build();

        Assert.Equal("?$filter=(Property eq 'Value' and Property2 eq 'Value2') or Property3 eq 'Value3'", query);
    }

    #endregion
}
