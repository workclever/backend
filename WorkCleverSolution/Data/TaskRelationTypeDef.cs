namespace WorkCleverSolution.Data;

// Later add color to visualize
public class TaskRelationTypeDef : TimeAwareEntity
{
    public string Type  { get; set; }
    public string InwardOperationName  { get; set; }
    public string OutwardOperationName  { get; set; }
}