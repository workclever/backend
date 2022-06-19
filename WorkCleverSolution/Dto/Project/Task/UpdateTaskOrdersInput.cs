namespace WorkCleverSolution.Dto.Project.Task;

public class UpdateTaskOrdersInput
{
    public Dictionary<int, List<int>> GroupedTasks { get; set; }
}