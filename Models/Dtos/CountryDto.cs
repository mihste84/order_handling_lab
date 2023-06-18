namespace Models.Dtos;

public record CountryDto(
    int? Id,
    string? Name,
    string? Abbreviation,
    string? PhonePrefix
);