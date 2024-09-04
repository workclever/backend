using System.ComponentModel.DataAnnotations;
using WorkCleverSolution.Attributes.Validation;

namespace WorkCleverSolution.Dto.Project.Task;

public class UpdateTaskPropertyInput
{
    [Required] [ValidTaskId] public int TaskId { get; set; }

    [Required] public string Property { get; set; }

    public string? Value { get; set; }
}