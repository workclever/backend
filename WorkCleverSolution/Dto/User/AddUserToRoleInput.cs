using System.ComponentModel.DataAnnotations;
using WorkCleverSolution.Attributes.Validation;

namespace WorkCleverSolution.Dto.User;

public class AddUserToRoleInput
{
    [Required] [ValidUserId] public int UserId { get; set; }

    public string[] Roles { get; set; }
}