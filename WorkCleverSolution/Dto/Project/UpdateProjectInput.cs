using System.ComponentModel.DataAnnotations;
using WorkCleverSolution.Attributes.Authorization;
using WorkCleverSolution.Attributes.Validation;

namespace WorkCleverSolution.Dto.Project;

public class UpdateProjectInput
{
    [Required]
    [ValidProjectId]
    [ManageableProjectId]
    public int ProjectId { get; set; }

    [Required]
    [MaxLength(20), MinLength(3)]
    public string Name { get; set; }

    [MaxLength(4), MinLength(1)]
    [Required]
    public string Slug { get; set; }
}