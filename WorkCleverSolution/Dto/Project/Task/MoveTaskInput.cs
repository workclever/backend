using System.ComponentModel.DataAnnotations;
using WorkCleverSolution.Attributes.Validation;

namespace WorkCleverSolution.Dto.Project.Task;

public class MoveTaskInput
{
    [Required] [ValidTaskId] public int TaskId { get; set; }
    [Required] [ValidBoardId] public int TargetBoardId { get; set; }
    [Required] [ValidColumnId] public int TargetColumnId { get; set; }
}