namespace OdataQueryBuilder.Test;

public class LogicalOperatorTest
{
    [Fact(DisplayName = "And test")]
    public void Test1()
    {
        var operation = "Property".And("Value");

        Assert.Equal("Property and Value", operation);
    }
    
    [Fact(DisplayName = "Or test")]
    public void Test2()
    {
        var operation = "Property".Or("Value");

        Assert.Equal("Property or Value", operation);
    }
}
