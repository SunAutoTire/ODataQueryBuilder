namespace OdataQueryBuilder.Test;

public class ComparisonOperatorTest
{
    [Fact(DisplayName = "Equal test")]
    public void Test1()
    {
        var operation = "Property".Equal("Value");

        Assert.Equal("Property eq 'Value'", operation);
    }
    
    [Fact(DisplayName = "Not Equal test")]
    public void Test2()
    {
        var operation = "Property".NotEqual("Value");

        Assert.Equal("Property ne 'Value'", operation);
    }
    
    [Fact(DisplayName = "Greater Than test")]
    public void Test3()
    {
        var operation = "Property".GreaterThan("Value");

        Assert.Equal("Property gt 'Value'", operation);
    }
    
    [Fact(DisplayName = "Greater Than Or Equal test")]
    public void Test4()
    {
        var operation = "Property".GreaterThanOrEqual("Value");

        Assert.Equal("Property ge 'Value'", operation);
    }
    
    [Fact(DisplayName = "Less Than test")]
    public void Test5()
    {
        var operation = "Property".LessThan("Value");

        Assert.Equal("Property lt 'Value'", operation);
    }
    
    [Fact(DisplayName = "Has test")]
    public void Test6()
    {
        var operation = "Style".Has("Sales.Color'Yellow'");

        Assert.Equal("Style has Sales.Color'Yellow'", operation);
    }
    
    [Fact(DisplayName = "In test")]
    public void Test7()
    {
        var operation = "City".In("Redmond","London");

        Assert.Equal("City in ('Redmond','London')", operation);
    }
}
