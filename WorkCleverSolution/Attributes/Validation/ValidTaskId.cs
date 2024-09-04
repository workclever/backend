using System.ComponentModel.DataAnnotations;
using WorkCleverSolution.Extensions;
using WorkCleverSolution.Services;

namespace WorkCleverSolution.Attributes.Validation;

public class ValidTaskId : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var taskId = (int) value;
        if (taskId <= 0)
        {
            return new ValidationResult("Please select a task");
        }

        var taskService = validationContext.GetService<ITaskService>();
        var projectService = validationContext.GetService<IProjectService>();
        var task = taskService.GetByIdInternal(taskId).Result;

        if (task == null)
        {
            return new ValidationResult("Selected task doesn't exist");
        }

        var httpContextAccessor = (IHttpContextAccessor) validationContext.GetService(typeof(IHttpContextAccessor));

        // Check access on project of task
        var hasAccess = projectService.HasAccess(httpContextAccessor.HttpContext.User, task.ProjectId).Result;
        if (!hasAccess)
        {
            return new ValidationResult("User doesn't have access to the selected task");
        }

        return ValidationResult.Success;
    }
}