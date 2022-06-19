namespace WorkCleverSolution.Data;

public class Column : TimeAwareEntity
{
    public string Name { get; set; }

    public int ProjectId { get; set; }
    public int BoardId { get; set; }
    public int UserId { get; set; }
    public bool Hidden { get; set; }
    public int Order { get; set; }
    public string Color { get; set; }
}