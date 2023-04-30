namespace AppLogic.Customers.CommonModels;

public class CustomerAddressModel
{
    public int? Id { get; set; }
    public string? Address { get; set; }
    public bool? IsPrimary { get; set; }
    public string? PostArea { get; set; }
    public string? ZipCode { get; set; }
    public int? CountryId { get; set; }
    public int? CityId { get; set; }
    public byte[]? RowVersion { get; set; }
}