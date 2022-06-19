using System.Security.Claims;
using WorkCleverSolution.Data.Identity;

namespace WorkCleverSolution.Extensions;

public static class UserExtensions
{
    public static int GetUserId(this ClaimsPrincipal identity)
    {
        var userId = Int32.Parse(identity.FindFirstValue(ClaimTypes.NameIdentifier));
        return userId;
    }
    
    public static bool IsAdmin(this ClaimsPrincipal identity)
    {
        return identity.IsInRole(Roles.Admin);
    }
}