using System.ComponentModel.DataAnnotations.Schema;

namespace WorkCleverSolution.Data;

public class TaskItem : TimeAwareEntity
{
    public string Title { get; set; }

    public int? ParentTaskItemId { get; set; }
    public TaskItem? ParentTaskItem { get; set; }

    public string Description { get; set; }

    public Project Project { get; set; }
    public int ProjectId { get; set; }
    public int BoardId { get; set; }
    public int ColumnId { get; set; }
    public int? ReporterUserId { get; set; }
    public int? AssigneeUserId { get; set; }
    public int Order { get; set; }
}