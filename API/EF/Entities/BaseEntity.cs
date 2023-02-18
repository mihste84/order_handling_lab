namespace API.EF.Entities;

public abstract class BaseEntity
{
    public long? Id { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? Created { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? Updated { get; set; }
}