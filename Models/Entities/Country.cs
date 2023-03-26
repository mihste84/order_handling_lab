namespace Models.Entities;

// Create C# class Country that inherits from BaseEntity
// and add the following properties: Name, Abbreviation, PhonePrefix, Customers
public class Country : BaseEntity
{
    public string? Name { get; set; }
    public string? Abbreviation { get; set; }
    public string? PhonePrefix { get; set; }
    public ICollection<City>? Cities { get; set; } = new HashSet<City>();
    public ICollection<CustomerAddress>? CustomerAddresses { get; set; } = new HashSet<CustomerAddress>();
}