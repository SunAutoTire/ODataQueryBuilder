
namespace OdataQueryBuilder.Test;

public class SelectTest
{
    #region String Arguments

    [Fact(DisplayName = "Select Single")]
    public void Test1()
    {
        var query = new QueryBuilder()
            .Select("Property")
            .Build();

        Assert.Equal("?$select=Property", query);
    }

    [Theory(DisplayName = "Select Single")]
    [InlineData(false, "Property")]
    [InlineData(true, "Property")]
    public void Test4(bool ignore, string select)
    {
        var query = new QueryBuilder()
            .Select(ignore, select)
            .Build();

        if (ignore)
            Assert.Equal(string.Empty, query);
        else
            Assert.Equal("?$select=" + select, query);
    }

    [Fact(DisplayName = "Select And")]
    public void Test2()
    {
        var query = new QueryBuilder()
            .Select("Property")
            .Select("Property2")
            .Build();

        Assert.Equal("?$select=Property,Property2", query);
    }

    [Fact(DisplayName = "Select With Filter")]
    public void Test3()
    {
        var query = new QueryBuilder()
            .Select("Property")
            .Filter("Property eq 'Value'")
            .Select("Property2")
            .Build();

        Assert.Equal("?$filter=Property eq 'Value'&$select=Property,Property2", query);
    }

    #endregion

    // NOTE: this region looks like a copy of FilterTest's "Explicit Arguments" region with Filter
    // renamed to Select: every case calls .Select(...) but asserts a "?$filter=..." result, and chains
    // .And()/.Or()/.ToBool(), which are $filter connectives with no meaning for $select. Commented out
    // pending a decision on what Select was meant to do here.
    // #region Explicit Arguments

    // [Fact(DisplayName = "Select Single")]
    // public void Test5()
    // {
    //     var query = new QueryBuilder()
    //         .Select("Property".Equal("Value"))
    //         .Build();

    //     Assert.Equal("?$filter=Property eq 'Value'", query);
    // }

    // [Fact(DisplayName = "Select And")]
    // public void Test6()
    // {
    //     var query = new QueryBuilder()
    //         .Select("Property".Equal("Value"))
    //         .And()
    //         .Select("Property2".Equal("Value2"))
    //         .Build();

    //     Assert.Equal("?$filter=Property eq 'Value' and Property2 eq 'Value2'", query);
    // }
    // [Fact(DisplayName = "Select Or")]

    // public void Test7()
    // {
    //     var query = new QueryBuilder()
    //         .Select("Property".Equal("Value"))
    //         .Or()
    //         .Select("Property2".Equal("Value2"))
    //         .Build();

    //     Assert.Equal("?$filter=Property eq 'Value' or Property2 eq 'Value2'", query);
    // }

    // [Fact(DisplayName = "Select And Or")]
    // public void Test8()
    // {
    //     var query = new QueryBuilder()
    //         .Select("Property".Equal("Value"))
    //         .And()
    //         .Select("Property2".Equal("Value2"))
    //         .ToBool()
    //         .Or()
    //         .Select("Property3".Equal("Value3"))
    //         .Build();

    //     Assert.Equal("?$filter=(Property eq 'Value' and Property2 eq 'Value2') or Property3 eq 'Value3'", query);
    // }

    // #endregion
}
