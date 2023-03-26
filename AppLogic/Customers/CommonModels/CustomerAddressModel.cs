namespace AppLogic.Customers.CommonModels;

public class CustomerAddressModel
{
    public string? Address { get; set; }
    public bool Primary { get; set; }
    public string? PostArea { get; set; }
    public string? ZipCode { get; set; }
    public int? CountryId { get; set; }
    public int? CityId { get; set; }
}