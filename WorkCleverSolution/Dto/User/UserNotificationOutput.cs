namespace WorkCleverSolution.Dto.User;

public class UserNotificationOutput
{
    public int Id { get; set; }
    public DateTime? DateCreated { get; set; }
    public int ByUserId { get; set; }
    public string Type { get; set; }
    public string Content { get; set; }

    public int? TaskId { get; set; }
}