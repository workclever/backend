using System.ComponentModel.DataAnnotations;

namespace WorkCleverSolution.Dto.User;

public class UpdateUserPreferenceInput
{
    [Required] public string Property { get; set; }
    public string Value { get; set; }
}