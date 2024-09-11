using System.ComponentModel.DataAnnotations;
using WorkCleverSolution.Attributes.Validation;

namespace WorkCleverSolution.Dto.Project.Board;

public class CreateBoardViewInput
{
    [Required] public string Type { get; set; }

    [Required]
    [ValidBoardId]
    public int BoardId { get; set; }
}