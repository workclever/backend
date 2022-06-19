using System.ComponentModel.DataAnnotations;
using WorkCleverSolution.Attributes.Authorization;
using WorkCleverSolution.Attributes.Validation;

namespace WorkCleverSolution.Dto.Project.Board;

public class UpdateBoardInput
{
    [Required]
    [ValidBoardId]
    [ManageableBoardId]
    public int BoardId { get; set; }

    [Required] public string Name { get; set; }
}