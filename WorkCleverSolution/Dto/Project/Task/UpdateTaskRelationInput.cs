using System.ComponentModel.DataAnnotations;
using WorkCleverSolution.Attributes.Validation;

namespace WorkCleverSolution.Dto.Project.Task;

public class UpdateTaskRelationInput
{
    [Required] [ValidTaskParentRelationId] public int TaskParentRelationId { get; set; }
    public int RelationTypeDefId { get; set; }
}