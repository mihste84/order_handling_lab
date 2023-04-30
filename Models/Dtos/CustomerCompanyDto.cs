namespace Models.Dtos;

public record CustomerCompanyDto
{
    public int? Id { get; set; }
    public int? CustomerId { get; set; }
    public string? Code { get; set; }
    public string? Type { get; set; }
    public bool Active { get; set; }
    public IEnumerable<CustomerAddressDto>? CustomerAddresses { get; set; }
    public IEnumerable<CustomerContactInfoDto>? CustomerContactInfos { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public byte[]? RowVersion { get; set; }
}