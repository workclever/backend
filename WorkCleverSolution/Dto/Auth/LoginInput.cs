using System.ComponentModel.DataAnnotations;

namespace WorkCleverSolution.Dto.Auth
{
    public class LoginInput
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}