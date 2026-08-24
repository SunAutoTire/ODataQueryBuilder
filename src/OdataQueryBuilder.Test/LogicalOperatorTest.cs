namespace OdataQueryBuilder.Test;

public class LogicalOperatorTest
{
    [Fact(DisplayName = "And")]
    public void Test1()
    {
        var operation = "Property".And("Value");

        Assert.Equal("Property and Value", operation.ToString());
    }
    
    [Fact(DisplayName = "Or")]
    public void Test2()
    {
        var operation = "Property".Or("Value");

        Assert.Equal("Property or Value", operation.ToString());
    }
}
