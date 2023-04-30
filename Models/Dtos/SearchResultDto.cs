namespace Models.Dtos;

public record SearchResultDto<T>(
    int TotalCount, 
    IEnumerable<T> Items, 
    int StartRow, 
    int EndRow, 
    string OrderBy, 
    string OrderByDirection
);