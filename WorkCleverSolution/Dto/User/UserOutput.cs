namespace WorkCleverSolution.Dto.User;

public class UserOutput
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public bool EmailConfirmed { get; set; }
    public string AvatarUrl { get; set; }
    public IList<string> Roles { get; set; }
    public UserPreferenceOutput Preferences { get; set; }
}