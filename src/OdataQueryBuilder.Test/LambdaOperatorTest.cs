
namespace OdataQueryBuilder.Test;

public class LambdaOperatorTest
{
    #region Path

    [Fact(DisplayName = "Path Navigates Into A Complex Type")]
    public void Test1()
    {
        Assert.Equal("Address/City eq 'Oslo'", "Address".Path("City").Equal("Oslo").ToString());
    }

    [Fact(DisplayName = "Path Takes Several Segments")]
    public void Test2()
    {
        Assert.Equal("Order/Customer/Name", "Order".Path("Customer", "Name").ToString());
    }

    [Fact(DisplayName = "Path Reaches The Count Segment")]
    public void Test3()
    {
        Assert.Equal("Items/$count gt 5", "Items".Path("$count").GreaterThan(5).ToString());
    }

    #endregion

    #region Any And All

    [Fact(DisplayName = "Any Without A Predicate Tests For Members")]
    public void Test4()
    {
        Assert.Equal("Items/any()", Any("Items").ToString());
    }

    [Fact(DisplayName = "Any Takes A Prebuilt Predicate")]
    public void Test5()
    {
        var operation = Any("Items", "i", new Expression("i/Price").GreaterThan(100));

        Assert.Equal("Items/any(i: i/Price gt 100)", operation.ToString());
    }

    [Fact(DisplayName = "Any Binds The Range Variable For A Lambda")]
    public void Test6()
    {
        var operation = Any("Items", "i", i => i.Path("Price").GreaterThan(100));

        Assert.Equal("Items/any(i: i/Price gt 100)", operation.ToString());
    }

    [Fact(DisplayName = "All Binds The Range Variable For A Lambda")]
    public void Test7()
    {
        var operation = All("Items", "i", i => i.Path("Qty").GreaterThan(0));

        Assert.Equal("Items/all(i: i/Qty gt 0)", operation.ToString());
    }

    [Fact(DisplayName = "Lambdas Nest With Distinct Range Variables")]
    public void Test8()
    {
        var operation = Any("Orders", "o", o => Any(o.Path("Items"), "i", i => i.Path("Price").GreaterThan(100)));

        Assert.Equal("Orders/any(o: o/Items/any(i: i/Price gt 100))", operation.ToString());
    }

    [Fact(DisplayName = "Lambda Result Needs No Parentheses As An Operand")]
    public void Test9()
    {
        var operation = Any("Items", "i", i => i.Path("Price").GreaterThan(100))
            .And("IsActive".Equal(true));

        Assert.Equal("Items/any(i: i/Price gt 100) and IsActive eq true", operation.ToString());
    }

    [Fact(DisplayName = "A Lambda Body Composes Freely")]
    public void Test10()
    {
        var operation = Any("Items", "i", i => i.Path("Price").GreaterThan(100).And(Contains(i.Path("Name"), "Milk")));

        Assert.Equal("Items/any(i: i/Price gt 100 and contains(i/Name,'Milk'))", operation.ToString());
    }

    [Fact(DisplayName = "Lambdas Reach The Query String")]
    public void Test11()
    {
        var query = new QueryBuilder("http://example.com", "Orders")
            .Filter(Any("Items", "i", i => i.Path("Price").GreaterThan(100)))
            .Build();

        Assert.Equal("http://example.com/Orders?$filter=Items/any(i: i/Price gt 100)", query);
    }

    #endregion
}
