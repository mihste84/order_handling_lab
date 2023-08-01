namespace Models.Dtos;

public record MasterDataDto(
    IEnumerable<CountryDto>? Countries,
    IEnumerable<CityDto>? Cities
);