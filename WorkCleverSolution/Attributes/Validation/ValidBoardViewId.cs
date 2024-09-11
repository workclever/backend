using System.ComponentModel.DataAnnotations;
using WorkCleverSolution.Extensions;
using WorkCleverSolution.Services;

namespace WorkCleverSolution.Attributes.Validation;

public class ValidBoardViewId : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var boardViewId = (int) value;
        if (boardViewId <= 0)
        {
            return new ValidationResult("Please select a board view");
        }

        var boardViewService = validationContext.GetService<IBoardViewService>();
        var boardView = boardViewService.GetById(boardViewId).Result;

        if (boardView == null)
        {
            return new ValidationResult("Selected board view doesn't exist");
        }

        var httpContextAccessor = (IHttpContextAccessor) validationContext.GetService(typeof(IHttpContextAccessor));

        // Check access on project of board
        var hasAccess = httpContextAccessor.HttpContext.User.GetUserId() == boardView.UserId;
        if (!hasAccess)
        {
            return new ValidationResult("User doesn't own the selected board view");
        }

        return ValidationResult.Success;
    }
}