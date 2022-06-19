using WorkCleverSolution.Attributes.Validation;

namespace WorkCleverSolution.Dto.Project.Task;

public class CreateTaskRelationInput
{
    [ValidTaskId] public int BaseTaskId { get; set; }
    [ValidTaskId] public int TargetTaskId { get; set; }
    public int RelationTypeDefId { get; set; }
}