namespace Models.Dtos;

public record CustomerContactInfoDto(
    int? Id,
    int? CustomerId,
    string? Type,
    string? Value,
    string? Prefix,
    byte[]? RowVersion
);