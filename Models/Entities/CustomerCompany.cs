namespace Models.Entities;

public class CustomerCompany : BaseEntity
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public int? CustomerId { get; set; }

}