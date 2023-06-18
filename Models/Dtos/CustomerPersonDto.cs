namespace Models.Dtos;

public record CustomerPersonDto
{
    public int? Id { get; set; }
    public int? CustomerId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public string? Ssn { get; set; }
    public bool Active { get; set; }
    public IEnumerable<CustomerAddressDto>? CustomerAddresses { get; set; }
    public IEnumerable<CustomerContactInfoDto>? CustomerContactInfos { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? Updated { get; set; }
    public byte[]? RowVersion { get; set; }
    public byte[]? CustomerRowVersion { get; set; }

}