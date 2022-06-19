using System.ComponentModel.DataAnnotations;

namespace WorkCleverSolution.Dto.Project;

public class CreateProjectInput
{
    [Required] public string Name { get; set; }

    [MaxLength(4), MinLength(1)]
    [Required] public string Slug { get; set; }
}