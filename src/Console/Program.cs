// See https://aka.ms/new-console-template for more information
using SunAuto.OData;

Console.WriteLine("Hello, World!");

var test = new QueryBuilder("https://sun.auto", "v1", "odata")
    .Select("Id", "Name")
    // .Expand("Orders", o => o.Select("Id", "Total"))
    .ToString();

Console.WriteLine(test);


test = new QueryBuilder("https://sun.auto", "v1", "odata")
    .Select("Id", "Name".Expand("Orders"))
    // .Expand("Orders", o => o.Select("Id", "Total"))
    .ToString();

Console.WriteLine(test);