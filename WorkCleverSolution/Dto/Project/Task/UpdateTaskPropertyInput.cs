using System.ComponentModel.DataAnnotations;
using WorkCleverSolution.Attributes.Validation;

namespace WorkCleverSolution.Dto.Project.Task;

public class UpdateTaskPropertyInput
{
    [Required] [ValidTaskId] public int TaskId { get; set; }

    [Required] public List<UpdateTaskPropertyInputParam> Params { get; set; }
}

public class UpdateTaskPropertyInputParam
{
    [Required] public string Property { get; set; }

    public string? Value { get; set; }
}