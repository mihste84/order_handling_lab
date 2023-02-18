namespace API.EF.Entities;


public class Workflow : BaseEntity
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    public long? WorkflowActivitiesId { get; set; }
    public WorkflowActivities? Activities { get; set; }
}