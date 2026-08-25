namespace OdataQueryBuilder.Test;

public class OdataQueryTests
{
    [Fact(DisplayName = "Route With No Options")]
    public void Test1()
    {
        var query = new QueryBuilder("http://example.com")
            .Build();

        Assert.Equal("http://example.com", query);
    }

    [Fact(DisplayName = "Route With Top And Skip")]
    public void Test2()
    {
        var query = new QueryBuilder("http://example.com")
            .Top(5)
            .Skip(2)
            .Build();

        Assert.Equal("http://example.com?$top=5&$skip=2", query);
    }

    [Fact(DisplayName = "Route With Repeated Filter, Skip And OrderBy")]
    public void Test3()
    {
        var query = new QueryBuilder("http://example.com")
            .Filter("jack".Equal("beanstalk"))
            .Skip(2)
            .Filter("redridinghood".NotEqual("wolf"))
            .OrderBy("jack")
            .Build();

        Assert.Equal("http://example.com?$filter=jack eq 'beanstalk' and redridinghood ne 'wolf'&$orderby=jack&$skip=2", query);
    }

    [Fact(DisplayName = "All Options With Nested Select")]
    public void Test4()
    {
        var query = new QueryBuilder("http://example.com")
            .Filter("jack".Equal("beanstalk").And("redridinghood".NotEqual("wolf")))
            .Skip(2)
            .Select("grandma")
            .Top(5)
            .Count()
            .OrderBy("jack")
            .Select("bears".Expand("parent").OrderByDescending("name").Expand("children"))
            .Build();

        Assert.Equal("http://example.com?$filter=jack eq 'beanstalk' and redridinghood ne 'wolf'&$select=grandma,bears($expand=parent;$orderby=name desc;$expand=children)&$orderby=jack&$top=5&$skip=2&$count=true", query);
    }
}
