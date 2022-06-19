using System.ComponentModel.DataAnnotations;

namespace WorkCleverSolution.Dto.Global;

public class CreateTaskRelationTypeDefInput
{
    [Required] public string Type { get; set; }
    [Required] public string InwardOperationName { get; set; }
    [Required] public string OutwardOperationName { get; set; }
}