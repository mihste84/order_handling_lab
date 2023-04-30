namespace Models.Dtos;

public record CustomerContactInfoDto(
    int? Id,
    int? CustomerId,
    string? Type,
    string? Value,
    string? CreatedBy,
    string? UpdatedBy,
    DateTime Created,
    DateTime Updated,
    byte[]? RowVersion
);