namespace API.EF.Entities;


public class WorkflowActivities : BaseEntity
{
    public string? Content { get; set; }
    public long? WorkflowId { get; set; }
    public Workflow? Workflow { get; set; }
}