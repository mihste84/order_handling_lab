namespace Models.Entities;

public class CustomerContactInfo : BaseEntity
{
    public string? Type { get; set; }
    public string? Value { get; set; }

    public int? CustomerId { get; set; }
}