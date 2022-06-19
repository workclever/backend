using System.ComponentModel.DataAnnotations;
using WorkCleverSolution.Attributes.Authorization;
using WorkCleverSolution.Attributes.Validation;

namespace WorkCleverSolution.Dto.Project.Board;

public class CreateBoardInput
{
    [Required] public string Name { get; set; }

    [Required]
    [ValidProjectId]
    [ManageableProjectId]
    public int ProjectId { get; set; }
}