using System.ComponentModel.DataAnnotations;
using WorkCleverSolution.Extensions;
using WorkCleverSolution.Services;

namespace WorkCleverSolution.Attributes.Validation;

public class ValidColumnId : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var columnId = (int) value;
        if (columnId <= 0)
        {
            return new ValidationResult("Please select a board column");
        }

        var columnService = validationContext.GetService<IColumnService>();
        var projectService = validationContext.GetService<IProjectService>();
        var column = columnService.GetById(columnId).Result;

        if (column == null)
        {
            return new ValidationResult("Selected board column doesn't exist");
        }

        var httpContextAccessor = (IHttpContextAccessor) validationContext.GetService(typeof(IHttpContextAccessor));

        // Check access on project of board
        var hasAccess = projectService.HasAccess(httpContextAccessor.HttpContext.User, column.ProjectId).Result;
        if (!hasAccess)
        {
            return new ValidationResult("User doesn't have access to the selected board column");
        }

        return ValidationResult.Success;
    }
}