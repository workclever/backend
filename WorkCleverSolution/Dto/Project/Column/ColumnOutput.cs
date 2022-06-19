namespace WorkCleverSolution.Dto.Project.Column;

public class ColumnOutput
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public int BoardId { get; set; }
    public bool Hidden { get; set; }
    public int Order { get; set; }
    public string Color { get; set; }
}