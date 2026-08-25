
namespace OdataQueryBuilder.Test;

public class LiteralTest
{
    #region Null

    [Fact(DisplayName = "Null Is The Null Literal")]
    public void Test1()
    {
        Assert.Equal("Name eq null", "Name".Equal(null).ToString());
        Assert.Equal("Name eq null", "Name".Equal(Expression.Null).ToString());
    }

    [Fact(DisplayName = "Null Is Distinct From The Empty String")]
    public void Test2()
    {
        Assert.Equal("Name eq ''", "Name".Equal(string.Empty).ToString());
    }

    [Fact(DisplayName = "Absent Nullable Is The Null Literal")]
    public void Test3()
    {
        Assert.Equal("Count eq null", "Count".Equal((int?)null).ToString());
        Assert.Equal("Count eq 5", "Count".Equal((int?)5).ToString());
    }

    #endregion

    #region Edm Primitives

    [Fact(DisplayName = "Guid Is Unquoted")]
    public void Test4()
    {
        var id = Guid.Parse("01234567-89ab-cdef-0123-456789abcdef");

        Assert.Equal("Id eq 01234567-89ab-cdef-0123-456789abcdef", "Id".Equal(id).ToString());
    }

    [Fact(DisplayName = "DateTimeOffset At UTC Ends In Z")]
    public void Test5()
    {
        var instant = new DateTimeOffset(2024, 1, 2, 3, 4, 5, TimeSpan.Zero);

        Assert.Equal("CreatedOn gt 2024-01-02T03:04:05Z", "CreatedOn".GreaterThan(instant).ToString());
    }

    [Fact(DisplayName = "DateTimeOffset Keeps A Non Zero Offset")]
    public void Test6()
    {
        var instant = new DateTimeOffset(2024, 1, 2, 3, 4, 5, TimeSpan.FromHours(2));

        Assert.Equal("CreatedOn gt 2024-01-02T03:04:05+02:00", "CreatedOn".GreaterThan(instant).ToString());
    }

    [Fact(DisplayName = "DateTimeOffset Keeps Fractional Seconds")]
    public void Test7()
    {
        var instant = new DateTimeOffset(2024, 1, 2, 3, 4, 5, 250, TimeSpan.Zero);

        Assert.Equal("CreatedOn gt 2024-01-02T03:04:05.25Z", "CreatedOn".GreaterThan(instant).ToString());
    }

    [Fact(DisplayName = "Date And TimeOfDay Are Unquoted")]
    public void Test8()
    {
        Assert.Equal("Day eq 2024-01-02", "Day".Equal(new DateOnly(2024, 1, 2)).ToString());
        Assert.Equal("Opens eq 13:20:00", "Opens".Equal(new TimeOnly(13, 20, 0)).ToString());
    }

    [Fact(DisplayName = "Duration Carries Its Prefix")]
    public void Test9()
    {
        Assert.Equal("Elapsed gt duration'PT1H30M'", "Elapsed".GreaterThan(TimeSpan.FromMinutes(90)).ToString());
    }

    [Fact(DisplayName = "Binary Uses The Url Safe Alphabet")]
    public void Test10()
    {
        Assert.Equal("Data eq binary'-___'", "Data".Equal(new byte[] { 0xFB, 0xFF, 0xFF }).ToString());
        Assert.Equal("Data eq binary'-_8='", "Data".Equal(new byte[] { 0xFB, 0xFF }).ToString());
    }

    [Fact(DisplayName = "Enum Member Is Type Qualified")]
    public void Test11()
    {
        var operation = "Style".Has(Expression.EnumMember("Sales.Color", "Yellow"));

        Assert.Equal("Style has Sales.Color'Yellow'", operation.ToString());
    }

    [Fact(DisplayName = "Boolean Is A Bare Keyword")]
    public void Test12()
    {
        Assert.Equal("IsActive eq true", "IsActive".Equal(true).ToString());
        Assert.Equal("IsActive ne false", "IsActive".NotEqual(false).ToString());
    }

    #endregion

    #region Numeric Specials

    [Fact(DisplayName = "Floating Point Specials Use OData Spelling")]
    public void Test13()
    {
        Assert.Equal("F eq NaN", "F".Equal(double.NaN).ToString());
        Assert.Equal("F eq INF", "F".Equal(double.PositiveInfinity).ToString());
        Assert.Equal("F eq -INF", "F".Equal(double.NegativeInfinity).ToString());
    }

    #endregion

    #region In A Query

    [Fact(DisplayName = "Literals Reach The Query String Unquoted")]
    public void Test14()
    {
        var query = new QueryBuilder("http://example.com")
            .Filter("DeletedOn".Equal(null))
            .Filter("CreatedOn".GreaterThan(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)))
            .Build();

        Assert.Equal(
            "http://example.com?$filter=DeletedOn eq null and CreatedOn gt 2024-01-01T00:00:00Z",
            query);
    }

    #endregion
}
