using System.ComponentModel.DataAnnotations;
using WorkCleverSolution.Attributes.Validation;

namespace WorkCleverSolution.Dto.User;

public class CreateUserInput
{
    [Required] public string FullName { get; set; }
    [Required] public string Email { get; set; }
    [Required] public string Password { get; set; }
}