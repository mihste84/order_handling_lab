namespace Models.Entities;

public class City : BaseEntity
{
    public string? Name { get; set; }
    public int? CountryId { get; set; }
    public Country? Country { get; set; }
    public ICollection<CustomerAddress>? CustomerAddresses { get; set; } = new HashSet<CustomerAddress>();
}