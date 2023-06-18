namespace Models.Dtos;

public record CityDto(
    int? Id,
    string? Name,
    int? CountryId
);