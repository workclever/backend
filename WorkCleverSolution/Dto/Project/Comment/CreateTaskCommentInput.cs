using System.ComponentModel.DataAnnotations;
using WorkCleverSolution.Attributes.Validation;

namespace WorkCleverSolution.Dto.Project.Comment;

public class CreateTaskCommentInput
{
    [Required] [ValidTaskId] public int TaskId { get; set; }
    [Required] public string Content { get; set; }
}