# ODataQueryBuilder

Build OData v4 / 4.01 query strings in .NET with a fluent interface.

Targets `net8.0`, `net9.0` and `net10.0`.

```
dotnet add package ODataQueryBuilder
```

```csharp
using SunAuto.OData;
using static SunAuto.OData.Functions;

var query = new QueryBuilder("https://example.com", "Products")
    .Filter("Price".GreaterThan(10).And(Contains(ToLower("Name"), "milk")))
    .Select("Name", "Price")
    .Expand("Category".Select("Name"))
    .OrderByDescending("Price")
    .Top(10)
    .Count()
    .Build();
```

```
https://example.com/Products?$filter=Price gt 10 and contains(tolower(Name),'milk')&$select=Name,Price&$expand=Category($select=Name)&$orderby=Price desc&$top=10&$count=true
```

This is a string builder, not a LINQ provider. It never sees your CLR types — you name properties as strings, and it handles the quoting, precedence, ordering and encoding.

## Literals and expressions

The one rule worth learning. A `string` argument is an **OData string literal** and is quoted. An `Expression` — which every operator and function returns — is emitted **verbatim**.

```csharp
"Name".Equal("Milk")                    // Name eq 'Milk'
"Total".Equal("Price".Multiply("Qty"))  // Total eq Price mul Qty
```

That means operators compose without any escape hatch: nothing can accidentally quote another operator's output. Other .NET values become the literal form OData's URL syntax expects, which is often not what `ToString()` gives you:

```csharp
"CreatedOn".GreaterThan(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero))
                                        // CreatedOn gt 2024-01-01T00:00:00Z   (unquoted)
"Id".Equal(someGuid)                    // Id eq 01234567-89ab-cdef-0123-456789abcdef
"DeletedOn".Equal(null)                 // DeletedOn eq null   (not '')
```

Covered: `string`, all numeric types, `bool`, `Guid`, `DateTimeOffset`, `DateTime`, `DateOnly`, `TimeOnly`, `TimeSpan` (as `duration'…'`), `byte[]` (as `binary'…'`), their nullable forms, `NaN`/`INF`/`-INF`, and enum members via `Expression.EnumMember("Sales.Color", "Yellow")`.

Build a literal explicitly with `Expression.Literal(text)`, or wrap hand-written syntax with `new Expression("…")`.

## Operators

| Group | Methods |
| --- | --- |
| Comparison | `Equal` `NotEqual` `GreaterThan` `GreaterThanOrEqual` `LessThan` `LessThanOrEqual` `Has` `In` |
| Logical | `And` `Or` `Not` |
| Arithmetic | `Add` `Subtract` `Multiply` `Divide` `DivideBy` `Modulo` |
| Other | `Group` `Path` `As` |

Arithmetic works over properties, numbers, or both — `"Price".Multiply("Qty")`, `"Price".Multiply(2)`, `100.Subtract("Discount")`.

**Parentheses appear only where they change meaning:**

```csharp
"a".Or("b").And("c")   // (a or b) and c   — regrouped, so parenthesised
"a".And("b").Or("c")   // a and b or c     — already correct, left alone
"a".And("b").Group()   // (a and b)        — Group() forces them
```

One limit: a raw `string` is treated as atomic, because the library does not parse it. `new Expression("a or b").And("c")` gives `a or b and c`. Build with operators, or `Group()` it yourself.

## Functions

The OData canonical functions are **static**, not extension methods, because most of their names (`Contains`, `StartsWith`, `ToLower`, `Substring`, `IndexOf`, `Trim`) are already instance methods on `string` and would be silently shadowed there — `"Name".Contains("Milk")` would quietly evaluate to a `bool`.

```csharp
using static SunAuto.OData.Functions;

Contains(ToLower("Name"), "milk")   // contains(tolower(Name),'milk')
Year("CreatedOn").Equal(2024)       // year(CreatedOn) eq 2024
```

Available: `Concat` `Contains` `EndsWith` `IndexOf` `Length` `MatchesPattern` `StartsWith` `Substring` `ToLower` `ToUpper` `Trim` `HasSubset` `HasSubsequence` · `Year` `Month` `Day` `Hour` `Minute` `Second` `FractionalSeconds` `Date` `Time` `TotalOffsetMinutes` `TotalSeconds` `Now` `MaxDateTime` `MinDateTime` · `Round` `Floor` `Ceiling` · `Cast` `IsOf` · `GeoDistance` `GeoLength` `GeoIntersects`

### Lambda operators

`Any` and `All` bind a range variable, so its name is written once rather than embedded in strings:

```csharp
Any("Items", "i", i => i.Path("Price").GreaterThan(100))   // Items/any(i: i/Price gt 100)
All("Items", "i", i => i.Path("Qty").GreaterThan(0))       // Items/all(i: i/Qty gt 0)
Any("Items")                                               // Items/any()
```

They nest — the bound variable is an ordinary `Expression`, so `Any(o.Path("Items"), "i", …)` works inside another lambda.

## Query options

`Filter` `Select` `Expand` `OrderBy` `OrderByDescending` `Compute` `Search` `Top` `Skip` `Count` `Apply` `Format` `Index` `SchemaVersion` `Id`

Most have an `…If(condition, …)` form taking a **positive** condition. `Count` is the exception, since its `bool` argument already carries one:

```csharp
.Select("Name").SelectIf(includeDetail, "Description")
```

**Options render in a fixed order, not call order**, so the same query always produces the same string — worth having for cache keys, logs and assertions:

```
$id · $apply · $compute · $filter · $search · $select · $expand · $orderby · $top · $skip · $count · $index · $schemaversion · $format
```

Each option appears at most once, as OData requires. `$select`, `$expand`, `$orderby` and `$compute` accumulate; the scalars replace; and **repeated `Filter` calls are joined with `and`**, parenthesised where precedence needs it:

```csharp
.Filter("Price".GreaterThan(10))
.Filter("InStock".Equal(true))       // ?$filter=Price gt 10 and InStock eq true
```

### Nested options

Scope options to an expanded property. They chain, and render in call order separated by `;`:

```csharp
.Expand("Items".Select("Name").Filter("Price".GreaterThan(100)).Top(5).Count(true))
// ?$expand=Items($select=Name;$filter=Price gt 100;$top=5;$count=true)
```

Available on both `string` and `OptionValue`: `Select` `Expand` `Filter` `OrderBy` `OrderByDescending` `Compute` `Top` `Skip` `Count` `Search` `Levels` `LevelsMax`.

Nested `Count` requires its argument — `"Items".Count()` would bind to `Enumerable.Count` and count the string's characters, so the no-argument call is deliberately unavailable.

### `$compute`

Define an alias, then use it from `$filter`, `$orderby` and `$select`:

```csharp
.Compute("Price".Multiply("Qty").As("Total"))
.Filter("Total".GreaterThan(100))
.OrderByDescending("Total")
// ?$compute=Price mul Qty as Total&$filter=Total gt 100&$orderby=Total desc
```

## Routes and keys

```csharp
new QueryBuilder("https://example.com", "Products").Key(1).Segment("Category")
// https://example.com/Products(1)/Category

.Key("Milk")                  // Products('Milk')      string keys are quoted
.Key("Name", "Milk")          // Products(Name='Milk')
.Key(("CategoryId", 1), ("Name", Expression.Literal("Milk")))
```

Use `Segment` for the path-only resources: `Segment("$count")`, `Segment("$value")`, `Segment("$ref")`.

## `$apply`

The Data Aggregation extension — optional, and not every service implements it.

```csharp
using static SunAuto.OData.Transformations;

.Apply(GroupBy(["Category"], Aggregate(Sum("Amount", "Total"), Count("Orders"))))
// ?$apply=groupby((Category),aggregate(Amount with sum as Total,$count as Orders))
```

Aggregate methods `Sum` `Min` `Max` `Average` `CountDistinct` `Count`; transformations `Aggregate` `GroupBy` `Filter` `Compute` `Concat` `Sequence` `Identity` `TopCount` `BottomCount` `TopPercent` `BottomPercent` `TopSum` `BottomSum`. Repeated `Apply` calls chain with `/`.

`nest`, `outernest`, `addnested` and the `expand` transformation are not wrapped; pass them as `new Expression("…")`.

## Parameter aliases

```csharp
.Filter("Name".Equal(Expression.Parameter("p1")))
.Parameter("p1", "Milk")
// ?$filter=Name eq @p1&@p1='Milk'
```

Aliases render after the system options. Redeclaring one replaces it.

## URLs

String literals percent-encode the characters that would otherwise corrupt the query string, so a value containing `&` cannot truncate the option it sits in:

```csharp
"Name".Equal("Milk & Honey")   // Name eq 'Milk %26 Honey'
```

`Build()` keeps spaces readable and is equivalent to `ToString()`. `ToUri()` returns a properly escaped `Uri`.

```csharp
builder.Build()            // https://example.com/Products?$filter=Name eq 'Milk'
builder.ToUri().AbsoluteUri // https://example.com/Products?$filter=Name%20eq%20'Milk'
```

Raw text passed as `new Expression("…")` is trusted verbatim — it is neither encoded nor parsed for precedence.

## A note on the `Expression` name

`SunAuto.OData.Expression` collides with `System.Linq.Expressions.Expression`. A file importing both namespaces will not compile:

```
error CS0104: 'Expression' is an ambiguous reference
```

Alias one of them until this is resolved:

```csharp
using Expression = SunAuto.OData.Expression;
```

## Not covered

Batch requests, `$skiptoken` / `$deltatoken` (server-generated), and typed property selectors.

## License

MIT
