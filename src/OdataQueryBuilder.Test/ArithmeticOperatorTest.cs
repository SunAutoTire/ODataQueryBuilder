namespace OdataQueryBuilder.Test;

public class ArithmeticOperatorTest
{
    #region Integer Arguments

    [Fact(DisplayName = "Add test")]
    public void Test1()
    {
        var operation = 1.Add(2);

        Assert.Equal("1 add 2", operation);
    }
    
    [Fact(DisplayName = "Subtract test")]
    public void Test2()
    {
        var operation = 2.Subtract(1);

        Assert.Equal("2 sub 1", operation);
    }
    
    [Fact(DisplayName = "Multiply test")]
    public void Test3()
    {
        var operation = 2.Multiply(3);

        Assert.Equal("2 mul 3", operation);
    }
    
    [Fact(DisplayName = "Divide test")]
    public void Test4()
    {
        var operation = 6.Divide(2);

        Assert.Equal("6 div 2", operation);
    }
    
    [Fact(DisplayName = "Divide test")]
    public void Test4()
    {
        var operation = 6.DivideBy(2.0);

        Assert.Equal("6 divby 2.0", operation);
    }
    
    [Fact(DisplayName = "Modulo test")]
    public void Test6   ()
    {
        var operation = 5.Modulo(2);

        Assert.Equal("5 mod 2", operation);
    }

    #endregion

     #region Double Arguments

    [Fact(DisplayName = "Add test")]
    public void Test7()
    {
        var operation = 1.1.Add(2.2);

        Assert.Equal("1.1 add 2.2", operation);
    }
    
    [Fact(DisplayName = "Subtract test")]
    public void Test8()
    {
        var operation = 2.2.Subtract(1.1);

        Assert.Equal("2.2 sub 1.1", operation);
    }
    
    [Fact(DisplayName = "Multiply test")]
    public void Test9()
    {
        var operation = 2.2.Multiply(3.3);

        Assert.Equal("2.2 mul 3.3", operation);
    }
    
    [Fact(DisplayName = "Divide test")]
    public void Test10()
    {
        var operation = 6.6.Divide(2.2);

        Assert.Equal("6.6 div 2.2", operation);
    }
    
    [Fact(DisplayName = "Divide By test")]
    public void Test11()
    {
        var operation = 6.0.DivideBy(3);

        Assert.Equal("6.0 divby 3", operation);
    }
    
    [Fact(DisplayName = "Modulo test")]
    public void Test12()
    {
        var operation = 5.5.Modulo(2.2);

        Assert.Equal("5.5 mod 2.2", operation);
    }

    #endregion
   
}
