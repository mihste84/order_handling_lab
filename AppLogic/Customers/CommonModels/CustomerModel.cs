namespace Customers.CommonModels;

public class CustomerModel
{
    public bool IsCompany { get; init; }
    public string? Code { get; init; }
    public string? Name { get; init; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Ssn { get; set; }
    public string? MiddleName { get; set; }
}