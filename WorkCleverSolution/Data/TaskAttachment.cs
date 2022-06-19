namespace WorkCleverSolution.Data;

public class TaskAttachment : TimeAwareEntity
{
    public string Name { get; set; }
    public string AttachmentUrl { get; set; }
    public int TaskId { get; set; }
}