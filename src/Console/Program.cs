// See https://aka.ms/new-console-template for more information
using SunAuto.OData;

Console.WriteLine("Hello, World!");

// var test = new QueryBuilder("https://sun.auto", "v1", "odata")
//     .Select("Id", "Name")
//     // .Expand("Orders", o => o.Select("Id", "Total"))
//     .ToString();

// Console.WriteLine(test);

var test = new QueryBuilder("https://sun.auto", "v1", "odata")
    .Select("Id", "Abbreviation","ArrayProperty".Expand("SubProperty1", "SubProperty2"), "ArrayProperty2".Expand("Label"))
    // .Expand("Orders", o => o.Select("Id", "Total"))
    .ToString();

    // https://sun.auto/v1/odata?$select=Id,Abbreviation,ArrayProperty($expand=SubProperty1,SubProperty2),ArrayProperty2($expand=Label)

Console.WriteLine(test);

// test = new QueryBuilder("https://sun.auto", "v1", "odata")
//     .Select(["Id", "Abbreviation"], o =>
//     {
//         o.Expand("Name");
//         o.Expand("Label");
//         o.OrderBy("Name", "Label");
//     })
//     // .Expand("Orders", o => o.Select("Id", "Total"))
//     .ToString();

// Console.WriteLine(test);