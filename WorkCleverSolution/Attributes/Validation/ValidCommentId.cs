using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using WorkCleverSolution.Data;

namespace WorkCleverSolution.Attributes.Validation;

public class ValidCommentId : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var commentId = (int) value;
        if (commentId <= 0)
        {
            return new ValidationResult("Please select a comment");
        }

        var dbContext = validationContext.GetService<ApplicationDbContext>();
        var comment = dbContext.TaskComments.Where(r => r.Id == commentId).SingleOrDefaultAsync().Result;

        if (comment == null)
        {
            return new ValidationResult("Selected comment doesn't exist");
        }

        return ValidationResult.Success;
    }
}