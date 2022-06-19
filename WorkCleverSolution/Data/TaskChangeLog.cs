namespace WorkCleverSolution.Data;

public class TaskChangeLog : TimeAwareEntity
{
    public string Property { get; set; }
    public string OldValue { get; set; }
    public string NewValue { get; set; }

    public int TaskId { get; set; }
    public int BoardId { get; set; }
    public int ProjectId { get; set; }
    public int UserId { get; set; }
}