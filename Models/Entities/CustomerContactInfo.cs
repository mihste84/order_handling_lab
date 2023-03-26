using Models.Enums;

namespace Models.Entities;

public class CustomerContactInfo : BaseEntity
{
    public ContactInfoType? Type { get; set; }
    public string? Value { get; set; }

    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }
}