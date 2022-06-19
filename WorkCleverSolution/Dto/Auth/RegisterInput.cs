using System.ComponentModel.DataAnnotations;

namespace WorkCleverSolution.Dto.Auth
{
    public class RegisterInput
    {
        [Required] public string FullName { get; set; }

        [Required] public string Email { get; set; }

        [Required] public string Password { get; set; }
    }
}