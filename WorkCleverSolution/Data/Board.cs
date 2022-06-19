namespace WorkCleverSolution.Data;

public class Board : TimeAwareEntity
{
    public string Name { get; set; }

    public int ProjectId { get; set; }
    public int UserId { get; set; }
}