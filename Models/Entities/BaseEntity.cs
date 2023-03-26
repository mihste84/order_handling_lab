namespace Models.Entities;

public abstract class BaseEntity
{
    public int? Id { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? Created { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? Updated { get; set; }
}