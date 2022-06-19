using System.ComponentModel.DataAnnotations;
using WorkCleverSolution.Attributes.Validation;

namespace WorkCleverSolution.Dto.User;

public class UpdateUserInput
{
    [Required] [ValidUserId] public int UserId { get; set; }
    [Required] public string FullName { get; set; }
}