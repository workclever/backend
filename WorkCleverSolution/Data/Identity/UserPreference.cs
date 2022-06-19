namespace WorkCleverSolution.Data.Identity;

public class UserPreference : TimeAwareEntity
{
    public int UserId { get; set; }
    public User User { get; set; }

    public string Timezone { get; set; }
}