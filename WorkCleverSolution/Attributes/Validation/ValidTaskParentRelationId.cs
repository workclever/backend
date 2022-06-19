using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using WorkCleverSolution.Data;

namespace WorkCleverSolution.Attributes.Validation;

public class ValidTaskParentRelationId : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var parentRelationId = (int) value;
        if (parentRelationId <= 0)
        {
            return new ValidationResult("Please select a parent relation");
        }

        var dbContext = validationContext.GetService<ApplicationDbContext>();
        var comment = dbContext.TaskParentRelations.Where(r => r.Id == parentRelationId).SingleOrDefaultAsync().Result;

        if (comment == null)
        {
            return new ValidationResult("Selected parent relation doesn't exist");
        }

        return ValidationResult.Success;
    }
}