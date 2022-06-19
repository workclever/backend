namespace WorkCleverSolution.Dto.Project.Comment;

public class TaskCommentOutput
{
    public int Id { get; set; }
    public string Content { get; set; }
    public int TaskId { get; set; }
    public int UserId { get; set; }
    public DateTime? DateCreated { get; set; }
}