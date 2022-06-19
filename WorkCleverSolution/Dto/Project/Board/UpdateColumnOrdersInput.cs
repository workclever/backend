namespace WorkCleverSolution.Dto.Project.Board;

public class UpdateColumnOrdersInput
{
    public int BoardId { get; set; }
    public List<int> ColumnIds { get; set; }
}