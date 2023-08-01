namespace Models.Entities;

public class Customer : BaseEntity
{
    public bool Active { get; set; }

    public CustomerCompany? CustomerCompany { get; set; }
    public CustomerPerson? CustomerPerson { get; set; }
    public ICollection<CustomerContactInfo>? CustomerContactInfos { get; set; } = new HashSet<CustomerContactInfo>();
    public ICollection<CustomerAddress>? CustomerAddresses { get; set; } = new HashSet<CustomerAddress>();
}
