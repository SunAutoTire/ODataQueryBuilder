namespace OdataQueryBuilder.Test;

public class ComputeTest
{
    #region As operator

    [Fact(DisplayName = "As Operator")]
    public void Test1()
    {
        var operation = "Price".Multiply("Qty").As("Total");

        Assert.Equal("Price mul Qty as Total", operation.ToString());
    }

    #endregion

    #region $compute

    [Fact(DisplayName = "Compute Single")]
    public void Test2()
    {
        var query = new QueryBuilder()
            .Compute("Price".Multiply("Qty").As("Total"))
            .Build();

        Assert.Equal("?$compute=Price mul Qty as Total", query);
    }

    [Fact(DisplayName = "Compute Multiple In One Call")]
    public void Test3()
    {
        var query = new QueryBuilder()
            .Compute("Price".Multiply("Qty").As("Total"), "Length".Multiply("Width").As("Area"))
            .Build();

        Assert.Equal("?$compute=Price mul Qty as Total,Length mul Width as Area", query);
    }

    [Fact(DisplayName = "Compute Accumulates Across Calls")]
    public void Test4()
    {
        var query = new QueryBuilder()
            .Compute("Price".Multiply("Qty").As("Total"))
            .Compute("Length".Multiply("Width").As("Area"))
            .Build();

        Assert.Equal("?$compute=Price mul Qty as Total,Length mul Width as Area", query);
    }

    [Theory(DisplayName = "Compute Conditional")]
    [InlineData(true, "Price mul Qty as Total")]
    [InlineData(false, "Price mul Qty as Total")]
    public void Test5(bool include, string compute)
    {
        var query = new QueryBuilder()
            .ComputeIf(include, compute)
            .Build();

        if (include)
            Assert.Equal("?$compute=" + compute, query);
        else
            Assert.Equal(string.Empty, query);
    }

    #endregion

    #region Computed aliases used by other options

    [Fact(DisplayName = "Compute Then Filter On The Alias")]
    public void Test6()
    {
        var query = new QueryBuilder()
            .Compute("Price".Multiply("Qty").As("Total"))
            .Filter("Total".GreaterThan(100))
            .Build();

        Assert.Equal("?$compute=Price mul Qty as Total&$filter=Total gt 100", query);
    }

    [Fact(DisplayName = "Compute Then Select And Order By The Alias")]
    public void Test7()
    {
        var query = new QueryBuilder("http://example.com")
            .Compute("Price".Multiply("Qty").As("Total"))
            .Select("Name")
            .Select("Total")
            .OrderByDescending("Total")
            .Top(5)
            .Build();

        Assert.Equal("http://example.com?$compute=Price mul Qty as Total&$select=Name,Total&$orderby=Total desc&$top=5", query);
    }

    [Fact(DisplayName = "Compute Accumulates Regardless Of Call Position")]
    public void Test8()
    {
        var query = new QueryBuilder()
            .Compute("Price".Multiply("Qty").As("Total"))
            .Select("Total")
            .Compute("Length".Multiply("Width").As("Area"))
            .Build();

        Assert.Equal("?$compute=Price mul Qty as Total,Length mul Width as Area&$select=Total", query);
    }

    #endregion

    #region Nested $compute

    [Fact(DisplayName = "Compute Nested In Expand")]
    public void Test9()
    {
        var query = new QueryBuilder()
            .Expand("Items".Compute("Price".Multiply("Qty").As("Total")))
            .Build();

        Assert.Equal("?$expand=Items($compute=Price mul Qty as Total)", query);
    }

    [Fact(DisplayName = "Compute Nested Alongside Other Options")]
    public void Test10()
    {
        var query = new QueryBuilder()
            .Expand("Items"
                .Compute("Price".Multiply("Qty").As("Total"))
                .Filter("Total".GreaterThan(100))
                .OrderByDescending("Total"))
            .Build();

        Assert.Equal("?$expand=Items($compute=Price mul Qty as Total;$filter=Total gt 100;$orderby=Total desc)", query);
    }

    #endregion
}
