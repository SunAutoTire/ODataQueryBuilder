
using static SunAuto.OData.Transformations;

namespace OdataQueryBuilder.Test;

public class ApplyTest
{
    #region Aggregate Methods

    [Fact(DisplayName = "Aggregate Methods")]
    public void Test1()
    {
        Assert.Equal("Amount with sum as Total", Sum("Amount", "Total").ToString());
        Assert.Equal("Amount with min as Lowest", Min("Amount", "Lowest").ToString());
        Assert.Equal("Amount with max as Highest", Max("Amount", "Highest").ToString());
        Assert.Equal("Amount with average as Mean", Average("Amount", "Mean").ToString());
        Assert.Equal("Region with countdistinct as Regions", CountDistinct("Region", "Regions").ToString());
        Assert.Equal("$count as Orders", Count("Orders").ToString());
    }

    [Fact(DisplayName = "Aggregate Method Over An Expression")]
    public void Test2()
    {
        var aggregate = Sum("Price".Multiply("Qty"), "Total");

        Assert.Equal("Price mul Qty with sum as Total", aggregate.ToString());
    }

    #endregion

    #region Transformations

    [Fact(DisplayName = "Aggregate Alone")]
    public void Test3()
    {
        var query = new QueryBuilder()
            .Apply(Aggregate(Sum("Amount", "Total")))
            .Build();

        Assert.Equal("?$apply=aggregate(Amount with sum as Total)", query);
    }

    [Fact(DisplayName = "Aggregate Several Values")]
    public void Test4()
    {
        var query = new QueryBuilder()
            .Apply(Aggregate(Max("Price", "Highest"), Count("Products")))
            .Build();

        Assert.Equal("?$apply=aggregate(Price with max as Highest,$count as Products)", query);
    }

    [Fact(DisplayName = "GroupBy Without A Transformation")]
    public void Test5()
    {
        var query = new QueryBuilder()
            .Apply(GroupBy("Category", "Region"))
            .Build();

        Assert.Equal("?$apply=groupby((Category,Region))", query);
    }

    [Fact(DisplayName = "GroupBy With An Aggregate")]
    public void Test6()
    {
        var query = new QueryBuilder()
            .Apply(GroupBy(["Category".Path("Name")], Aggregate(Sum("Amount", "Total"))))
            .Build();

        Assert.Equal("?$apply=groupby((Category/Name),aggregate(Amount with sum as Total))", query);
    }

    [Fact(DisplayName = "Ranking Transformations")]
    public void Test7()
    {
        Assert.Equal("topcount(5,Amount)", TopCount(5, "Amount").ToString());
        Assert.Equal("bottomcount(5,Amount)", BottomCount(5, "Amount").ToString());
        Assert.Equal("toppercent(10,Amount)", TopPercent(10, "Amount").ToString());
        Assert.Equal("bottompercent(10,Amount)", BottomPercent(10, "Amount").ToString());
        Assert.Equal("topsum(100,Amount)", TopSum(100, "Amount").ToString());
        Assert.Equal("bottomsum(100,Amount)", BottomSum(100, "Amount").ToString());
        Assert.Equal("identity", Identity().ToString());
    }

    #endregion

    #region Chaining

    [Fact(DisplayName = "Transformations Chain With A Slash")]
    public void Test8()
    {
        var query = new QueryBuilder()
            .Apply(
                Transformations.Filter("Amount".GreaterThan(10)),
                GroupBy(["Category"], Aggregate(Average("Amount", "Mean"))))
            .Build();

        Assert.Equal("?$apply=filter(Amount gt 10)/groupby((Category),aggregate(Amount with average as Mean))", query);
    }

    [Fact(DisplayName = "Repeated Apply Chains Onto The Same Option")]
    public void Test9()
    {
        var query = new QueryBuilder()
            .Apply(Transformations.Filter("Amount".GreaterThan(10)))
            .Apply(Aggregate(Sum("Amount", "Total")))
            .Build();

        Assert.Equal("?$apply=filter(Amount gt 10)/aggregate(Amount with sum as Total)", query);
    }

    [Theory(DisplayName = "Apply Conditional")]
    [InlineData(true)]
    [InlineData(false)]
    public void Test10(bool include)
    {
        var query = new QueryBuilder()
            .ApplyIf(include, Aggregate(Sum("Amount", "Total")))
            .Build();

        Assert.Equal(include ? "?$apply=aggregate(Amount with sum as Total)" : string.Empty, query);
    }

    #endregion

    #region With Other Options

    [Fact(DisplayName = "Apply Precedes The Options It Feeds")]
    public void Test11()
    {
        var query = new QueryBuilder("http://example.com", "Orders")
            .OrderByDescending("Total")
            .Top(3)
            .Apply(GroupBy(["Category"], Aggregate(Sum("Amount", "Total"))))
            .Build();

        Assert.Equal(
            "http://example.com/Orders?$apply=groupby((Category),aggregate(Amount with sum as Total))"
                + "&$orderby=Total desc&$top=3",
            query);
    }

    #endregion
}
