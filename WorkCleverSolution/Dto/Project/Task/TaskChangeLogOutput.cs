namespace WorkCleverSolution.Dto.Project.Task;

public class TaskChangeLogOutput
{
    public int Id { get; set; }
    public string Property { get; set; }
    public string OldValue { get; set; }
    public string NewValue { get; set; }
    public DateTime? DateCreated { get; set; }
    
    public int UserId { get; set; }
}