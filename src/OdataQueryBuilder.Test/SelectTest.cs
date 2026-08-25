
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

    [Theory(DisplayName = "Select Conditional")]
    [InlineData(true, "Property")]
    [InlineData(false, "Property")]
    public void Test4(bool include, string select)
    {
        var query = new QueryBuilder()
            .SelectIf(include, select)
            .Build();

        if (include)
            Assert.Equal("?$select=" + select, query);
        else
            Assert.Equal(string.Empty, query);
    }

    [Fact(DisplayName = "Select Multiple")]
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
}
