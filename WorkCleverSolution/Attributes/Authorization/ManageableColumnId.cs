using System.ComponentModel.DataAnnotations;
using WorkCleverSolution.Data.Identity;
using WorkCleverSolution.Extensions;
using WorkCleverSolution.Services;

namespace WorkCleverSolution.Attributes.Authorization;

public class ManageableColumnId : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var columnId = (int) value;
        if (columnId <= 0)
        {
            return new ValidationResult("Please select a column");
        }

        var httpContextAccessor = (IHttpContextAccessor) validationContext.GetService(typeof(IHttpContextAccessor));
        var accessManagerService = validationContext.GetService<IUserEntityAccessManagerService>();
        var user = httpContextAccessor.HttpContext.User;
        if (user.IsAdmin())
        {
            return ValidationResult.Success;
        }

        var permission = Permissions.CanManageProject;
        var entityClass = "Project";
        var columnService = validationContext.GetService<IColumnService>();
        var column = columnService.GetById(columnId).Result;
        var projectId = column.ProjectId;
        var hasAccess = accessManagerService.HasUserAccessOnEntity(user.GetUserId(), permission, projectId, entityClass)
            .Result;
        
        if (hasAccess)
        {
            return ValidationResult.Success;
        }

        return new ValidationResult("User doesn't have the specific permission: " + permission);
    }
}