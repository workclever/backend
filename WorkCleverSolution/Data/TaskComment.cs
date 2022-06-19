namespace WorkCleverSolution.Data;

public class TaskComment : TimeAwareEntity
{
    public string Content { get; set; }
    public int TaskId { get; set; }
    public int UserId { get; set; }
}