namespace OdataQueryBuilder.Test;

public class ComparisonOperatorTest
{
    [Fact(DisplayName = "Equal")]
    public void Test1()
    {
        var operation = "Property".Equal("Value");

        Assert.Equal("Property eq 'Value'", operation.ToString());
    }
    
    [Fact(DisplayName = "Not Equal")]
    public void Test2()
    {
        var operation = "Property".NotEqual("Value");

        Assert.Equal("Property ne 'Value'", operation.ToString());
    }
    
    [Fact(DisplayName = "Greater Than")]
    public void Test3()
    {
        var operation = "Property".GreaterThan("Value");

        Assert.Equal("Property gt 'Value'", operation.ToString());
    }
    
    [Fact(DisplayName = "Greater Than Or Equal")]
    public void Test4()
    {
        var operation = "Property".GreaterThanOrEqual("Value");

        Assert.Equal("Property ge 'Value'", operation.ToString());
    }
    
    [Fact(DisplayName = "Less Than")]
    public void Test5()
    {
        var operation = "Property".LessThan("Value");

        Assert.Equal("Property lt 'Value'", operation.ToString());
    }
    
    [Fact(DisplayName = "Has")]
    public void Test6()
    {
        var operation = "Style".Has("Sales.Color'Yellow'");

        Assert.Equal("Style has Sales.Color'Yellow'", operation.ToString());
    }
    
    [Fact(DisplayName = "In")]
    public void Test7()
    {
        var operation = "City".In("Redmond","London");

        Assert.Equal("City in ('Redmond','London')", operation.ToString());
    }
}
