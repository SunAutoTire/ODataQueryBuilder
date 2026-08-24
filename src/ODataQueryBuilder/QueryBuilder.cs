// namespace SunAuto.OData;

// public class QueryBuilder(params string?[] routeSegments)
// {
//     public string Route = string.Join('/', routeSegments.Where(rs => !string.IsNullOrWhiteSpace(rs)));

//     public List<Option> Options { get; private set; } = [];

//     internal void Add(Option? option)
//     {
//         if (option is not null)
//             Options = [.. Options, option];
//     }

//     public override string ToString()
//     {
//         var options = string.Join('&', Options.Select(o => o.ToString()));
//         return $"{Route}?{options}";
//     }
// }
