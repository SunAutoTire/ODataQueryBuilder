
namespace OdataQueryBuilder.Test;

public class NestedOptionTest
{
    [Fact(DisplayName = "Nested Top And Skip")]
    public void Test1()
    {
        var query = new QueryBuilder()
            .Expand("Items".Top(5).Skip(2))
            .Build();

        Assert.Equal("?$expand=Items($top=5;$skip=2)", query);
    }

    [Fact(DisplayName = "Nested Count")]
    public void Test2()
    {
        var query = new QueryBuilder()
            .Expand("Items".Count(true))
            .Build();

        Assert.Equal("?$expand=Items($count=true)", query);
    }

    [Fact(DisplayName = "Nested Search")]
    public void Test3()
    {
        var query = new QueryBuilder()
            .Expand("Items".Search("milk"))
            .Build();

        Assert.Equal("?$expand=Items($search=milk)", query);
    }

    [Fact(DisplayName = "Nested Levels To A Depth")]
    public void Test4()
    {
        var query = new QueryBuilder()
            .Expand("Category".Levels(3))
            .Build();

        Assert.Equal("?$expand=Category($levels=3)", query);
    }

    [Fact(DisplayName = "Nested Levels To The Maximum")]
    public void Test5()
    {
        var query = new QueryBuilder()
            .Expand("Category".LevelsMax())
            .Build();

        Assert.Equal("?$expand=Category($levels=max)", query);
    }

    [Fact(DisplayName = "Nested Options Combine In Call Order")]
    public void Test6()
    {
        var query = new QueryBuilder()
            .Expand("Items"
                .Select("Name", "Price")
                .Filter("Price".GreaterThan(100))
                .OrderByDescending("Price")
                .Top(5)
                .Count(true))
            .Build();

        Assert.Equal(
            "?$expand=Items($select=Name,Price;$filter=Price gt 100;$orderby=Price desc;$top=5;$count=true)",
            query);
    }
}
