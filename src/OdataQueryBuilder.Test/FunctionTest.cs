
namespace OdataQueryBuilder.Test;

public class FunctionTest
{
    #region String Functions

    [Fact(DisplayName = "Contains Quotes A String Argument")]
    public void Test1()
    {
        Assert.Equal("contains(Name,'Milk')", Contains("Name", "Milk").ToString());
    }

    [Fact(DisplayName = "Contains Emits An Expression Argument Verbatim")]
    public void Test2()
    {
        Assert.Equal("contains(Name,Nickname)", Contains("Name", new Expression("Nickname")).ToString());
    }

    [Fact(DisplayName = "StartsWith And EndsWith")]
    public void Test3()
    {
        Assert.Equal("startswith(Name,'Mi')", StartsWith("Name", "Mi").ToString());
        Assert.Equal("endswith(Name,'lk')", EndsWith("Name", "lk").ToString());
    }

    [Fact(DisplayName = "IndexOf And Length")]
    public void Test4()
    {
        Assert.Equal("indexof(Name,'lk')", IndexOf("Name", "lk").ToString());
        Assert.Equal("length(Name)", Length("Name").ToString());
    }

    [Fact(DisplayName = "Substring Takes One Or Two Positions")]
    public void Test5()
    {
        Assert.Equal("substring(Name,1)", Substring("Name", 1).ToString());
        Assert.Equal("substring(Name,1,3)", Substring("Name", 1, 3).ToString());
    }

    [Fact(DisplayName = "Case And Whitespace Functions")]
    public void Test6()
    {
        Assert.Equal("tolower(Name)", ToLower("Name").ToString());
        Assert.Equal("toupper(Name)", ToUpper("Name").ToString());
        Assert.Equal("trim(Name)", Trim("Name").ToString());
    }

    [Fact(DisplayName = "Concat And MatchesPattern")]
    public void Test7()
    {
        Assert.Equal("concat(City,', ')", Concat("City", ", ").ToString());
        Assert.Equal("matchesPattern(Name,'^A.*')", MatchesPattern("Name", "^A.*").ToString());
    }

    [Fact(DisplayName = "Collection Subset Functions")]
    public void Test8()
    {
        Assert.Equal("hassubset(Tags,Wanted)", HasSubset("Tags", new Expression("Wanted")).ToString());
        Assert.Equal("hassubsequence(Tags,Wanted)", HasSubsequence("Tags", new Expression("Wanted")).ToString());
    }

    #endregion

    #region Date And Time Functions

    [Fact(DisplayName = "Date Component Functions")]
    public void Test9()
    {
        Assert.Equal("year(CreatedOn)", Year("CreatedOn").ToString());
        Assert.Equal("month(CreatedOn)", Month("CreatedOn").ToString());
        Assert.Equal("day(CreatedOn)", Day("CreatedOn").ToString());
    }

    [Fact(DisplayName = "Time Component Functions")]
    public void Test10()
    {
        Assert.Equal("hour(CreatedOn)", Hour("CreatedOn").ToString());
        Assert.Equal("minute(CreatedOn)", Minute("CreatedOn").ToString());
        Assert.Equal("second(CreatedOn)", Second("CreatedOn").ToString());
        Assert.Equal("fractionalseconds(CreatedOn)", FractionalSeconds("CreatedOn").ToString());
    }

    [Fact(DisplayName = "Date Time Projection Functions")]
    public void Test11()
    {
        Assert.Equal("date(CreatedOn)", Date("CreatedOn").ToString());
        Assert.Equal("time(CreatedOn)", Time("CreatedOn").ToString());
        Assert.Equal("totaloffsetminutes(CreatedOn)", TotalOffsetMinutes("CreatedOn").ToString());
        Assert.Equal("totalseconds(Elapsed)", TotalSeconds("Elapsed").ToString());
    }

    [Fact(DisplayName = "Nullary Date Functions")]
    public void Test12()
    {
        Assert.Equal("now()", Now().ToString());
        Assert.Equal("maxdatetime()", MaxDateTime().ToString());
        Assert.Equal("mindatetime()", MinDateTime().ToString());
    }

    #endregion

    #region Arithmetic, Type And Geo Functions

    [Fact(DisplayName = "Rounding Functions")]
    public void Test13()
    {
        Assert.Equal("round(Price)", Round("Price").ToString());
        Assert.Equal("floor(Price)", Floor("Price").ToString());
        Assert.Equal("ceiling(Price)", Ceiling("Price").ToString());
    }

    [Fact(DisplayName = "Cast And IsOf Take One Or Two Arguments")]
    public void Test14()
    {
        Assert.Equal("cast(Edm.String)", Cast("Edm.String").ToString());
        Assert.Equal("cast(Price,Edm.Decimal)", Cast("Price", "Edm.Decimal").ToString());
        Assert.Equal("isof(Sales.Manager)", IsOf("Sales.Manager").ToString());
        Assert.Equal("isof(Employee,Sales.Manager)", IsOf("Employee", "Sales.Manager").ToString());
    }

    [Fact(DisplayName = "Geo Functions")]
    public void Test15()
    {
        Assert.Equal("geo.distance(From,To)", GeoDistance("From", "To").ToString());
        Assert.Equal("geo.length(Route)", GeoLength("Route").ToString());
        Assert.Equal("geo.intersects(Point,Area)", GeoIntersects("Point", "Area").ToString());
    }

    #endregion

    #region Composition

    [Fact(DisplayName = "Functions Nest Inside Each Other")]
    public void Test16()
    {
        Assert.Equal("contains(tolower(Name),'milk')", Contains(ToLower("Name"), "milk").ToString());
    }

    [Fact(DisplayName = "Function Result Needs No Parentheses As An Operand")]
    public void Test17()
    {
        var operation = Length("Name").GreaterThan(5).And(Year("CreatedOn").Equal(2024));

        Assert.Equal("length(Name) gt 5 and year(CreatedOn) eq 2024", operation.ToString());
    }

    [Fact(DisplayName = "Function Arguments Keep Compound Expressions Bare")]
    public void Test18()
    {
        Assert.Equal("round(Price mul Qty)", Round("Price".Multiply("Qty")).ToString());
    }

    [Fact(DisplayName = "Functions Reach The Query String")]
    public void Test19()
    {
        var query = new QueryBuilder("http://example.com")
            .Filter(Contains(ToLower("Name"), "milk"))
            .Filter(Year("CreatedOn").Equal(2024))
            .OrderBy("Name")
            .Build();

        Assert.Equal(
            "http://example.com?$filter=contains(tolower(Name),'milk') and year(CreatedOn) eq 2024&$orderby=Name",
            query);
    }

    #endregion
}
