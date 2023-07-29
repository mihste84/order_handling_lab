namespace Models.Values;

public record SearchResult<M>(int TotalCount, IEnumerable<M> Items);