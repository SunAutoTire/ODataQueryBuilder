
namespace OdataQueryBuilder.Test;

public class SystemOptionTest
{
    #region Format

    [Fact(DisplayName = "Format Abbreviation")]
    public void Test1()
    {
        Assert.Equal("?$format=json", new QueryBuilder().Format("json").Build());
    }

    [Fact(DisplayName = "Format Media Type")]
    public void Test2()
    {
        var query = new QueryBuilder().Format("application/json;odata.metadata=full").Build();

        Assert.Equal("?$format=application/json;odata.metadata=full", query);
    }

    #endregion

    #region Index, SchemaVersion And Id

    [Fact(DisplayName = "Index")]
    public void Test3()
    {
        Assert.Equal("?$index=2", new QueryBuilder().Index(2).Build());
    }

    [Fact(DisplayName = "SchemaVersion")]
    public void Test4()
    {
        Assert.Equal("?$schemaversion=*", new QueryBuilder().SchemaVersion("*").Build());
        Assert.Equal("?$schemaversion=1.0", new QueryBuilder().SchemaVersion("1.0").Build());
    }

    [Fact(DisplayName = "Id Reads An Entity By Its Id")]
    public void Test5()
    {
        var query = new QueryBuilder("http://example.com")
            .Segment("$entity")
            .Id("http://example.com/Products(1)")
            .Select("Name")
            .Build();

        Assert.Equal("http://example.com/$entity?$id=http://example.com/Products(1)&$select=Name", query);
    }

    [Fact(DisplayName = "Id Accepts A Uri")]
    public void Test6()
    {
        var query = new QueryBuilder().Id(new Uri("http://example.com/Products(1)")).Build();

        Assert.Equal("?$id=http://example.com/Products(1)", query);
    }

    #endregion

    #region Conditional Forms

    [Theory(DisplayName = "System Options Conditional")]
    [InlineData(true)]
    [InlineData(false)]
    public void Test7(bool include)
    {
        var query = new QueryBuilder()
            .FormatIf(include, "json")
            .IndexIf(include, 2)
            .SchemaVersionIf(include, "*")
            .IdIf(include, "http://example.com/Products(1)")
            .Build();

        Assert.Equal(
            include ? "?$id=http://example.com/Products(1)&$index=2&$schemaversion=*&$format=json" : string.Empty,
            query);
    }

    #endregion

    #region Parameter Aliases

    [Fact(DisplayName = "Parameter Alias Holds A String Literal")]
    public void Test8()
    {
        var query = new QueryBuilder("http://example.com", "Products")
            .Filter("Name".Equal(Expression.Parameter("p1")))
            .Parameter("p1", "Milk")
            .Build();

        Assert.Equal("http://example.com/Products?$filter=Name eq @p1&@p1='Milk'", query);
    }

    [Fact(DisplayName = "Parameter Alias Holds An Expression")]
    public void Test9()
    {
        var query = new QueryBuilder()
            .Filter("Price".GreaterThan(Expression.Parameter("@min")))
            .Parameter("@min", 100)
            .Build();

        Assert.Equal("?$filter=Price gt @min&@min=100", query);
    }

    [Fact(DisplayName = "Parameter Aliases Render After The System Options")]
    public void Test10()
    {
        var query = new QueryBuilder()
            .Parameter("p1", "Milk")
            .Filter("Name".Equal(Expression.Parameter("p1")))
            .Top(5)
            .Build();

        Assert.Equal("?$filter=Name eq @p1&$top=5&@p1='Milk'", query);
    }

    [Fact(DisplayName = "Redeclaring A Parameter Alias Replaces It")]
    public void Test11()
    {
        var query = new QueryBuilder()
            .Parameter("p1", "Milk")
            .Parameter("p1", "Honey")
            .Build();

        Assert.Equal("?@p1='Honey'", query);
    }

    #endregion
}
