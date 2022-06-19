using System.ComponentModel.DataAnnotations;
using WorkCleverSolution.Attributes.Validation;

namespace WorkCleverSolution.Dto.Project.Comment;

public class DeleteTaskCommentInput
{
    [Required] [ValidTaskId] public int TaskId { get; set; }
    [Required] [ValidCommentId] public int CommentId { get; set; }
}