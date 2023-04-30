namespace AppLogic.Customers.CommonModels;

public class CustomerContactInfoModel
{
    public int? Id { get; set; }

    public string? Type { get; set; }
    public string? Value { get; set; }
    public byte[]? RowVersion { get; set; }
}