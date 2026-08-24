
namespace OdataQueryBuilder.Test;

public class ExpressionTest
{
    #region Literals Versus Expressions

    [Fact(DisplayName = "String Argument Is A Quoted Literal")]
    public void Test1()
    {
        var operation = "Name".Equal("Milk");

        Assert.Equal("Name eq 'Milk'", operation.ToString());
    }

    [Fact(DisplayName = "Expression Argument Is Not Quoted")]
    public void Test2()
    {
        var operation = "Total".Equal("Price".Multiply("Qty"));

        Assert.Equal("Total eq Price mul Qty", operation.ToString());
    }

    [Fact(DisplayName = "Number Argument Is A Numeric Literal")]
    public void Test3()
    {
        var operation = "Price".GreaterThan(100);

        Assert.Equal("Price gt 100", operation.ToString());
    }

    [Fact(DisplayName = "Boolean Argument Is A Boolean Literal")]
    public void Test4()
    {
        var operation = "IsActive".Equal(true);

        Assert.Equal("IsActive eq true", operation.ToString());
    }

    [Fact(DisplayName = "Quote In A Literal Is Doubled")]
    public void Test5()
    {
        var operation = "Name".Equal("O'Brien");

        Assert.Equal("Name eq 'O''Brien'", operation.ToString());
    }

    [Fact(DisplayName = "Computed Alias Compares Without Quoting")]
    public void Test6()
    {
        var operation = "Price".Multiply("Qty").GreaterThan(100);

        Assert.Equal("Price mul Qty gt 100", operation.ToString());
    }

    #endregion

    #region Precedence

    [Fact(DisplayName = "Disjunction Is Grouped Inside A Conjunction")]
    public void Test7()
    {
        var operation = "a".Or("b").And("c");

        Assert.Equal("(a or b) and c", operation.ToString());
    }

    [Fact(DisplayName = "Conjunction Needs No Group Inside A Disjunction")]
    public void Test8()
    {
        var operation = "a".And("b").Or("c");

        Assert.Equal("a and b or c", operation.ToString());
    }

    [Fact(DisplayName = "Comparisons Need No Group Inside A Conjunction")]
    public void Test9()
    {
        var operation = "a".Equal("x").And("b".Equal("y"));

        Assert.Equal("a eq 'x' and b eq 'y'", operation.ToString());
    }

    [Fact(DisplayName = "Addition Is Grouped Inside A Multiplication")]
    public void Test10()
    {
        var operation = "a".Add("b").Multiply("c");

        Assert.Equal("(a add b) mul c", operation.ToString());
    }

    [Fact(DisplayName = "Left Associative Chain Needs No Group")]
    public void Test11()
    {
        var operation = "a".Subtract("b").Subtract("c");

        Assert.Equal("a sub b sub c", operation.ToString());
    }

    [Fact(DisplayName = "Right Operand At Equal Precedence Is Grouped")]
    public void Test12()
    {
        var operation = "a".Subtract("b".Subtract("c"));

        Assert.Equal("a sub (b sub c)", operation.ToString());
    }

    [Fact(DisplayName = "Not Groups A Compound Operand")]
    public void Test13()
    {
        var operation = "a".Equal("x").Not();

        Assert.Equal("not (a eq 'x')", operation.ToString());
    }

    [Fact(DisplayName = "Not Leaves A Simple Operand Bare")]
    public void Test14()
    {
        var operation = "IsActive".Not();

        Assert.Equal("not IsActive", operation.ToString());
    }

    [Fact(DisplayName = "Group Forces Parentheses")]
    public void Test15()
    {
        var operation = "a".And("b").Group();

        Assert.Equal("(a and b)", operation.ToString());
    }

    [Fact(DisplayName = "Hand Written Text Is Treated As Atomic")]
    public void Test16()
    {
        // The library does not parse a raw string, so it cannot know this one binds loosely. Callers who
        // hand-write a disjunction must group it themselves before combining it with something tighter.
        var operation = new Expression("a or b").And("c");

        Assert.Equal("a or b and c", operation.ToString());
        Assert.Equal("(a or b) and c", new Expression("a or b").Group().And("c").ToString());
    }

    #endregion

    #region Url Safety

    [Fact(DisplayName = "Ampersand In A Literal Is Encoded")]
    public void Test17()
    {
        var operation = "Name".Equal("Milk & Honey");

        Assert.Equal("Name eq 'Milk %26 Honey'", operation.ToString());
    }

    [Fact(DisplayName = "Percent Is Encoded Before The Characters That Introduce It")]
    public void Test18()
    {
        var operation = "Name".Equal("100% & more");

        Assert.Equal("Name eq '100%25 %26 more'", operation.ToString());
    }

    [Fact(DisplayName = "Hash And Plus In A Literal Are Encoded")]
    public void Test19()
    {
        var operation = "Name".Equal("C# + F#");

        Assert.Equal("Name eq 'C%23 %2B F%23'", operation.ToString());
    }

    [Fact(DisplayName = "Structural Characters Are Left Alone")]
    public void Test20()
    {
        var operation = "Name".Equal("a,b(c)d");

        Assert.Equal("Name eq 'a,b(c)d'", operation.ToString());
    }

    #endregion
}
