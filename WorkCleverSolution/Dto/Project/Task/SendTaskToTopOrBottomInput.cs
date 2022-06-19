using WorkCleverSolution.Attributes.Validation;

namespace WorkCleverSolution.Dto.Project.Task;

public class SendTaskToTopOrBottomInput
{
    public int TaskId { get; set; }
    public int Location { get; set; } // 1 -> Top, 0 -> Bottom
}