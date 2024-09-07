using System.ComponentModel.DataAnnotations;
using WorkCleverSolution.Attributes.Validation;

namespace WorkCleverSolution.Dto.Project.Task;

public class UpdateTaskAssigneeUserInput
{
    [Required] [ValidTaskId] public int TaskId { get; set; }
    [Required] [ValidUserId] public List<int> UserIds { get; set; }
}