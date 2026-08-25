
namespace OdataQueryBuilder.Test;

public class RouteTest
{
    [Fact(DisplayName = "Segment Appends To The Route")]
    public void Test1()
    {
        var query = new QueryBuilder("http://example.com")
            .Segment("Products")
            .Build();

        Assert.Equal("http://example.com/Products", query);
    }

    [Fact(DisplayName = "Numeric Key")]
    public void Test2()
    {
        var query = new QueryBuilder("http://example.com", "Products")
            .Key(1)
            .Build();

        Assert.Equal("http://example.com/Products(1)", query);
    }

    [Fact(DisplayName = "String Key Is Quoted")]
    public void Test3()
    {
        var query = new QueryBuilder("http://example.com", "Products")
            .Key("Milk")
            .Build();

        Assert.Equal("http://example.com/Products('Milk')", query);
    }

    [Fact(DisplayName = "Named Key")]
    public void Test4()
    {
        Assert.Equal(
            "http://example.com/Products(Id=1)",
            new QueryBuilder("http://example.com", "Products").Key("Id", 1).Build());

        Assert.Equal(
            "http://example.com/Products(Name='Milk')",
            new QueryBuilder("http://example.com", "Products").Key("Name", "Milk").Build());
    }

    [Fact(DisplayName = "Composite Key")]
    public void Test5()
    {
        var query = new QueryBuilder("http://example.com", "Products")
            .Key(("CategoryId", 1), ("Name", Expression.Literal("Milk")))
            .Build();

        Assert.Equal("http://example.com/Products(CategoryId=1,Name='Milk')", query);
    }

    [Fact(DisplayName = "Key Then Navigation Segment")]
    public void Test6()
    {
        var query = new QueryBuilder("http://example.com", "Products")
            .Key(1)
            .Segment("Category")
            .Build();

        Assert.Equal("http://example.com/Products(1)/Category", query);
    }

    [Fact(DisplayName = "Path Only Resources")]
    public void Test7()
    {
        Assert.Equal(
            "http://example.com/Products/$count",
            new QueryBuilder("http://example.com", "Products").Segment("$count").Build());

        Assert.Equal(
            "http://example.com/Products(1)/Name/$value",
            new QueryBuilder("http://example.com", "Products").Key(1).Segment("Name", "$value").Build());
    }

    [Fact(DisplayName = "Key Combines With Query Options")]
    public void Test8()
    {
        var query = new QueryBuilder("http://example.com", "Products")
            .Key(1)
            .Select("Name")
            .Expand("Category")
            .Build();

        Assert.Equal("http://example.com/Products(1)?$select=Name&$expand=Category", query);
    }

    [Fact(DisplayName = "Key Without An Entity Set Is Rejected")]
    public void Test9()
    {
        Assert.Throws<InvalidOperationException>(() => new QueryBuilder().Key(1));
    }
}
