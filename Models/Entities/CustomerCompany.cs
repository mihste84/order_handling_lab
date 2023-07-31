namespace Models.Entities;

public class CustomerCompany
{
    public int? Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public int? CustomerId { get; set; }
}