using System.ComponentModel.DataAnnotations;
using WorkCleverSolution.Extensions;
using WorkCleverSolution.Services;

namespace WorkCleverSolution.Attributes.Validation;

public class ValidProjectId : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var projectId = (int) value;
        if (projectId <= 0)
        {
            return new ValidationResult("Please select a project");
        }

        var projectService = validationContext.GetService<IProjectService>();
        var project = projectService.GetById(projectId).Result;

        if (project == null)
        {
            return new ValidationResult("Selected project doesn't exist");
        }

        var httpContextAccessor = (IHttpContextAccessor) validationContext.GetService(typeof(IHttpContextAccessor));

        var hasAccess = projectService.HasAccess(httpContextAccessor.HttpContext.User, projectId).Result;
        if (!hasAccess)
        {
            return new ValidationResult("User doesn't have access to the selected project");
        }

        return ValidationResult.Success;
    }
}