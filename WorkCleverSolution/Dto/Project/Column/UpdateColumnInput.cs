using System.ComponentModel.DataAnnotations;
using WorkCleverSolution.Attributes.Validation;

namespace WorkCleverSolution.Dto.Project.Column;

public class UpdateColumnInput
{
    [Required] public string Name { get; set; }
    [Required] [ValidColumnId] public int ColumnId { get; set; }
    public bool Hidden { get; set; }
    [Required] public string Color { get; set; }
}