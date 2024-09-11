using System.ComponentModel.DataAnnotations;
using WorkCleverSolution.Attributes.Validation;

namespace WorkCleverSolution.Dto.Project.Board;

public class UpdateBoardViewInput
{
    [Required]
    [ValidBoardViewId]
    public int BoardViewId { get; set; }
    
    [Required] public string Name { get; set; }
    
    [Required]public List<int> VisibleCustomFields { get; set; }
}