namespace Models.Dtos;

public record SearchCustomerDto(
    int? Id,
    string? Name,
    string? Code,
    string? FirstName,
    string? LastName,
    string? MiddleName,
    string? Ssn,
    bool? Active,
    string? CreatedBy,
    string? UpdatedBy,
    DateTime? Created,
    DateTime? Updated,
    bool IsCompany
);