namespace WorkCleverSolution.Data;

public class TaskAssignee : TimeAwareEntity
{
    public int TaskId { get; set; }

    public int UserId { get; set; }
}