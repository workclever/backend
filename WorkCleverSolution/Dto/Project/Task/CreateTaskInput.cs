using System.ComponentModel.DataAnnotations;
using WorkCleverSolution.Attributes.Validation;

namespace WorkCleverSolution.Dto.Project.Task;

public class CreateTaskInput
{
    [Required] [ValidProjectId] public int ProjectId { get; set; }

    [Required] [ValidBoardId] public int BoardId { get; set; }
    [Required] [ValidColumnId] public int ColumnId { get; set; }

    [Required] public string Title { get; set; }

    public string Description { get; set; }

    [ValidUserId] public int ReporterUserId { get; set; }
    [ValidUserId] public int AssigneeUserId { get; set; }
    public int? ParentTaskItemId { get; set; }
}