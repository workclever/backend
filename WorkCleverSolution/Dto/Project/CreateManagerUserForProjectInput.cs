using System.ComponentModel.DataAnnotations;
using WorkCleverSolution.Attributes.Authorization;
using WorkCleverSolution.Attributes.Validation;

namespace WorkCleverSolution.Dto.Project;

public class CreateManagerUserForProjectInput
{
    [Required]
    [ValidProjectId]
    [ManageableProjectId]
    public int ProjectId { get; set; }

    [Required] [ValidUserId] public int UserId { get; set; }
}