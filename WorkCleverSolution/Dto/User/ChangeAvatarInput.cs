using System.ComponentModel.DataAnnotations;

namespace WorkCleverSolution.Dto.User;

public class ChangeAvatarInput
{
    [Required]
    public IFormFile file { get; set; }
}