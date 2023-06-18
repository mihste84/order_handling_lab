namespace Models.Dtos;

public record CustomerAddressDto(
    int? Id,
    int? CustomerId,
    bool? IsPrimary,
    string? Address,
    string? PostArea,
    string? ZipCode,
    int? CountryId,
    int? CityId,
    string? CreatedBy,
    string? UpdatedBy,
    DateTime? Created,
    DateTime? Updated,
    byte[]? RowVersion
);
