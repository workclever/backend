using System.ComponentModel.DataAnnotations;
using WorkCleverSolution.Attributes.Authorization;
using WorkCleverSolution.Attributes.Validation;

namespace WorkCleverSolution.Dto.Project;

public class CreateProjectAssigneeInput
{
    public class UserIdDto
    {
        [Required] [ValidUserId] public int UserId { get; set; }
    }

    [Required]
    [ValidProjectId]
    [ManageableProjectId]
    public int ProjectId { get; set; }

    [Required] public List<UserIdDto> Ids { get; set; }
}