using System.ComponentModel.DataAnnotations;
using WorkCleverSolution.Extensions;
using WorkCleverSolution.Services;

namespace WorkCleverSolution.Attributes.Validation;

public class ValidBoardId : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var boardId = (int) value;
        if (boardId <= 0)
        {
            return new ValidationResult("Please select a board");
        }

        var boardService = validationContext.GetService<IBoardService>();
        var projectService = validationContext.GetService<IProjectService>();
        var board = boardService.GetById(boardId).Result;

        if (board == null)
        {
            return new ValidationResult("Selected board doesn't exist");
        }

        var httpContextAccessor = (IHttpContextAccessor) validationContext.GetService(typeof(IHttpContextAccessor));

        // Check access on project of board
        var hasAccess = projectService.HasAccess(httpContextAccessor.HttpContext.User, board.ProjectId).Result;
        if (!hasAccess)
        {
            return new ValidationResult("User doesn't have access to the selected board");
        }

        return ValidationResult.Success;
    }
}