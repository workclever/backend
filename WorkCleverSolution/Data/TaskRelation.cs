namespace WorkCleverSolution.Data;

public class TaskRelation : TimeAwareEntity
{
    public int TaskId { get; set; }
    public int TargetTaskId { get; set; }
    public int UserId { get; set; }
    
    public int TaskParentRelationId { get; set; }
    public TaskParentRelation TaskParentRelation { get; set; }
    
    public string RelationTypeDirection { get; set; }
}