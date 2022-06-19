using System.ComponentModel.DataAnnotations;

namespace WorkCleverSolution.Dto.User;

public class ChangePasswordInput
{
    [Required] public string OldPassword { get; set; }
    [Required] public string NewPassword { get; set; }
}