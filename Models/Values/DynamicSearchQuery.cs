using Models.Constants;

namespace Models.Values;

public record SearchItem(string Name, object? Value, string Operator, bool IsBaseWhere = true);

public class DynamicSearchQuery
{
    public List<SearchItem> SearchItems { get; set; } = new();
    public int StartRow { get; set; } = 0;
    public int EndRow { get; set; } = 10;
    public string OrderBy { get; set; } = "Id";
    public string OrderByDirection { get; set; } = "ASC";

    public static FormattableString GetWhereFromSearchItem(SearchItem item) 
    => item.Operator switch {
        SearchOperators.Equal => ($"{item.Name:raw} = {item.Value}" ),
        SearchOperators.NotEqual => ($"{item.Name:raw} <> {item.Value}" ),
        SearchOperators.GreaterThan => ($"{item.Name:raw} > {item.Value}" ),
        SearchOperators.GreaterThanOrEqual => ($"{item.Name:raw} >= {item.Value}" ),
        SearchOperators.LessThan => ($"{item.Name:raw} < {item.Value}" ),
        SearchOperators.LessThanOrEqual => ($"{item.Name:raw} <= {item.Value}" ),
        SearchOperators.StartsWith => ($"{item.Name:raw} LIKE '{item.Value + "%"}'"),
        SearchOperators.EndsWith => ($"{item.Name:raw} LIKE '{"%" + item.Value}'"),
        SearchOperators.Contains => ($"{item.Name:raw} LIKE '{"%" + item.Value + "%"}'"),
        SearchOperators.In => ($"{item.Name:raw} IN {item.Value}"),
        SearchOperators.NotIn => ($"{item.Name:raw} NOT IN {item.Value}"),
        SearchOperators.IsNull => ($"{item.Name:raw} IS NULL"),
        SearchOperators.IsNotNull => ($"{item.Name:raw} IS NOT NULL"),
        _ => throw new ArgumentOutOfRangeException($"Operator '{item.Operator}' not supported.")
    };

    public static bool TryExtractSearchItemByName(IEnumerable<SearchItem> items, string name, out SearchItem? item) {
        item = items.FirstOrDefault(_ => _.Name == name);
        return item != null;
    }

}