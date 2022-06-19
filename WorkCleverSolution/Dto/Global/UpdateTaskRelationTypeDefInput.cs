using System.ComponentModel.DataAnnotations;

namespace WorkCleverSolution.Dto.Global;

public class UpdateTaskRelationTypeDefInput
{
    [Required] public int Id { get; set; }
    [Required] public string Type { get; set; }
    [Required] public string InwardOperationName { get; set; }
    [Required] public string OutwardOperationName { get; set; }
}