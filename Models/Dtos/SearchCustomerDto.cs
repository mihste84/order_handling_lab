namespace Models.Dtos;

public class SearchCustomerDto {
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public string? Ssn { get; set; }
    public bool? Active { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? Updated { get; set; }
    public bool IsCompany { get; set; }
}