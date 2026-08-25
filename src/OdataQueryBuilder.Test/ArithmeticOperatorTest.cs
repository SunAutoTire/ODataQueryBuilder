namespace OdataQueryBuilder.Test;

public class ArithmeticOperatorTest
{
    #region Integer Arguments

    [Fact(DisplayName = "Add Integers")]
    public void Test1()
    {
        var operation = 1.Add(2);

        Assert.Equal("1 add 2", operation.ToString());
    }
    
    [Fact(DisplayName = "Subtract Integers")]
    public void Test2()
    {
        var operation = 2.Subtract(1);

        Assert.Equal("2 sub 1", operation.ToString());
    }
    
    [Fact(DisplayName = "Multiply Integers")]
    public void Test3()
    {
        var operation = 2.Multiply(3);

        Assert.Equal("2 mul 3", operation.ToString());
    }
    
    [Fact(DisplayName = "Divide Integers")]
    public void Test4()
    {
        var operation = 6.Divide(2);

        Assert.Equal("6 div 2", operation.ToString());
    }
    
    [Fact(DisplayName = "Divide By Integer And Double")]
    public void Test5()
    {
        var operation = 6.DivideBy(2.0);

        Assert.Equal("6 divby 2.0", operation.ToString());
    }
    
    [Fact(DisplayName = "Modulo Integers")]
    public void Test6()
    {
        var operation = 5.Modulo(2);

        Assert.Equal("5 mod 2", operation.ToString());
    }

    #endregion

     #region Double Arguments

    [Fact(DisplayName = "Add Doubles")]
    public void Test7()
    {
        var operation = 1.1.Add(2.2);

        Assert.Equal("1.1 add 2.2", operation.ToString());
    }
    
    [Fact(DisplayName = "Subtract Doubles")]
    public void Test8()
    {
        var operation = 2.2.Subtract(1.1);

        Assert.Equal("2.2 sub 1.1", operation.ToString());
    }
    
    [Fact(DisplayName = "Multiply Doubles")]
    public void Test9()
    {
        var operation = 2.2.Multiply(3.3);

        Assert.Equal("2.2 mul 3.3", operation.ToString());
    }
    
    [Fact(DisplayName = "Divide Doubles")]
    public void Test10()
    {
        var operation = 6.6.Divide(2.2);

        Assert.Equal("6.6 div 2.2", operation.ToString());
    }
    
    [Fact(DisplayName = "Divide By Double And Integer")]
    public void Test11()
    {
        var operation = 6.0.DivideBy(3);

        Assert.Equal("6.0 divby 3", operation.ToString());
    }
    
    [Fact(DisplayName = "Modulo Doubles")]
    public void Test12()
    {
        var operation = 5.5.Modulo(2.2);

        Assert.Equal("5.5 mod 2.2", operation.ToString());
    }

    #endregion

    #region Property Arguments

    [Fact(DisplayName = "Add Properties")]
    public void Test13()
    {
        var operation = "Price".Add("Surcharge");

        Assert.Equal("Price add Surcharge", operation.ToString());
    }

    [Fact(DisplayName = "Subtract Properties")]
    public void Test14()
    {
        var operation = "Price".Subtract("Discount");

        Assert.Equal("Price sub Discount", operation.ToString());
    }

    [Fact(DisplayName = "Multiply Properties")]
    public void Test15()
    {
        var operation = "Price".Multiply("Qty");

        Assert.Equal("Price mul Qty", operation.ToString());
    }

    [Fact(DisplayName = "Divide Properties")]
    public void Test16()
    {
        var operation = "Total".Divide("Qty");

        Assert.Equal("Total div Qty", operation.ToString());
    }

    [Fact(DisplayName = "Divide By Properties")]
    public void Test17()
    {
        var operation = "Total".DivideBy("Qty");

        Assert.Equal("Total divby Qty", operation.ToString());
    }

    [Fact(DisplayName = "Modulo Properties")]
    public void Test18()
    {
        var operation = "Qty".Modulo("PackSize");

        Assert.Equal("Qty mod PackSize", operation.ToString());
    }

    #endregion

    #region Mixed Property And Number Arguments

    [Fact(DisplayName = "Multiply Property By Integer")]
    public void Test19()
    {
        var operation = "Price".Multiply(2);

        Assert.Equal("Price mul 2", operation.ToString());
    }

    [Fact(DisplayName = "Divide Property By Double")]
    public void Test20()
    {
        var operation = "Price".DivideBy(2.0);

        Assert.Equal("Price divby 2.0", operation.ToString());
    }

    [Fact(DisplayName = "Subtract Property From Integer")]
    public void Test21()
    {
        var operation = 100.Subtract("Discount");

        Assert.Equal("100 sub Discount", operation.ToString());
    }

    [Fact(DisplayName = "Add Property To Double")]
    public void Test22()
    {
        var operation = 1.5.Add("Price");

        Assert.Equal("1.5 add Price", operation.ToString());
    }

    #endregion

    #region Nested Arithmetic

    [Fact(DisplayName = "Compose Arithmetic Through Group")]
    public void Test23()
    {
        var operation = "Price".Subtract("Discount").Group().Multiply("Qty");

        Assert.Equal("(Price sub Discount) mul Qty", operation.ToString());
    }

    #endregion
}
