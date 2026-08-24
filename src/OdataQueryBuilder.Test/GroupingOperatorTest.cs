namespace OdataQueryBuilder.Test;

public class GroupingOperatorTest
{
    #region String Argument

    [Fact(DisplayName = "Group Property")]
    public void Test1()
    {
        var operation = "Property".Group();

        Assert.Equal("(Property)", operation.ToString());
    }

    #endregion

    #region Integer Argument

    [Fact(DisplayName = "Group Integer")]
    public void Test2()
    {
        var operation = 42.Group();

        Assert.Equal("(42)", operation.ToString());
    }

    #endregion

    #region Double Argument

    [Fact(DisplayName = "Group Double")]
    public void Test3()
    {
        var operation = 42.42.Group();

        Assert.Equal("(42.42)", operation.ToString());
    }

    #endregion
}
