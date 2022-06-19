using Microsoft.AspNetCore.Identity;

namespace WorkCleverSolution.Data.Identity;

public class User : IdentityUser<int>
{
    public string FullName { get; set; }
    public string AvatarUrl { get; set; }
}