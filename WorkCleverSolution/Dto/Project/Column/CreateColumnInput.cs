using System.ComponentModel.DataAnnotations;
using WorkCleverSolution.Attributes.Validation;

namespace WorkCleverSolution.Dto.Project.Column;

public class CreateColumnInput
{
    [Required] public string Name { get; set; }

    [Required] [ValidProjectId] public int ProjectId { get; set; }
    [Required] [ValidBoardId] public int BoardId { get; set; }
    public bool Hidden { get; set; }
}