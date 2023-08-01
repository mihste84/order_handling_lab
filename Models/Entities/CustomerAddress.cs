namespace Models.Entities;

public class CustomerAddress : BaseEntity
{
    public string? Address { get; set; }
    public bool? IsPrimary { get; set; }
    public string? PostArea { get; set; }
    public string? ZipCode { get; set; }
    public int? CountryId { get; set; }
    public Country? Country { get; set; }
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int? CityId { get; set; }
    public City? City { get; set; }
}