namespace Models.Entities;

public class CustomerPerson
{
    public int? Id { get; set; }
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string? Ssn { get; set; }
    public int? CustomerId { get; set; }
}