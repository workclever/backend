using WorkCleverSolution.Data.Identity;

namespace WorkCleverSolution.Data;

public class TaskAssignee : TimeAwareEntity
{
    public TaskItem Task { get; set; }
    public int TaskId { get; set; }

    public User User { get; set; }
    public int UserId { get; set; }
}