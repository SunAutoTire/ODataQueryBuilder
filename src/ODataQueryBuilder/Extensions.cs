namespace SunAuto.OData;

// public class Extensions
// {
//     public async Task Run()
//     {
//         var clause = new QueryBuilder("Id", "Name");

//         //(facility/name eq '4237' and date eq 2026-05-22)
//         //PreviousBusinessDay($expand=startingCashEntries($expand=CashDrawerSlots($expand=usDenomination)),endingCashEntries($expand=CashDrawerSlots($expand=usDenomination))),startingCashEntries($expand=CashDrawerSlots($expand=usDenomination)),endingCashEntries($expand=CashDrawerSlots($expand=usDenomination))

//         clause
//             .Filter("(facility/name".Eq("4237").And("date".Eq("2026-05-22")))
//             .Expand("PreviousBusinessDay", pbd => pbd
//                 .Expand("startingCashEntries", sce => sce
//                     .Expand("CashDrawerSlots", cds => cds.Expand("usDenomination")))
//                 .Expand("endingCashEntries", ece => ece
//                     .Expand("CashDrawerSlots", cds => cds.Expand("usDenomination"))))
//             .Expand("startingCashEntries", sce => sce
//                 .Expand("CashDrawerSlots", cds => cds.Expand("usDenomination")))
//             .Expand("endingCashEntries", ece => ece
//                 .Expand("CashDrawerSlots", cds => cds.Expand("usDenomination")));

//         Console.WriteLine(clause); // Output: $select=Id,Name
//     }
// }

internal static class Extensions
{
    public static Option Filter(this QueryBuilder builder, string filter) => builder.AddClause(new FilterClause(filter));
    public static Option Filter(this Option builder, string filter) => builder.AddClause(new FilterClause(filter));

    public static Option Expand(this QueryBuilder builder, string propertyName, Func<QueryBuilder, Option>? nested = null) => builder.AddClause(new ExpandClause(propertyName, nested?.Invoke(new QueryBuilder())));
    public static Option Expand(this Option builder, string propertyName, Func<QueryBuilder, Option>? nested = null) => builder.AddClause(new ExpandClause(propertyName, nested?.Invoke(new QueryBuilder())));

    public static string Eq(this string field, string value) => $"{field} eq '{value}'";
    public static string And(this string left, string right) => $"({left} and {right})";
}