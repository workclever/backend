using WorkCleverSolution.Data;

namespace WorkCleverSolution.Services;

public class UserNotification : TimeAwareEntity
{
    public int ByUserId { get; set; }
    public int ToUserId { get; set; }
    public string Type { get; set; }
    public string Content { get; set; }

    public int? TaskId { get; set; }
    public bool Unread { get; set; }
}