namespace WorkCleverSolution.Dto.Project.Task;

public class TaskOutput
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int BoardId { get; set; }
    public int ColumnId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int? ReporterUserId { get; set; }
    public int? AssigneeUserId { get; set; }
    public int? ParentTaskItemId { get; set; }
    public int Order { get; set; }
    public string Slug { get; set; }
}