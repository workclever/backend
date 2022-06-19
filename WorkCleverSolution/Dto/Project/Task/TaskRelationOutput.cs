namespace WorkCleverSolution.Dto.Project.Task;

public class TaskRelationOutput
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public int RelationTypeDefId { get; set; }
    public string RelationTypeDirection { get; set; }
    public int TaskParentRelationId { get; set; }
}