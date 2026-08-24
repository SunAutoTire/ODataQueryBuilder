namespace OdataQueryBuilder.Test;

public class GroupingOperatorTest
{
    #region String Argument

    [Fact(DisplayName = "Parenthesis test")]
    public void Test1()
    {
        var operation = "Property".Parenthesis();

        Assert.Equal("(Property)", operation);
    }

    #endregion

    #region Integer Argument

    [Fact(DisplayName = "Parenthesis test")]
    public void Test2()
    {
        var operation = 42.Parenthesis();

        Assert.Equal("(42)", operation);
    }

    #endregion

    #region Double Argument

    [Fact(DisplayName = "Parenthesis test")]
    public void Test3()
    {
        var operation = 42.42.Parenthesis();

        Assert.Equal("(42.42)", operation);
    }

    #endregion
}
