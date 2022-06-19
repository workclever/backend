using System.ComponentModel.DataAnnotations;
using WorkCleverSolution.Data;

namespace WorkCleverSolution.Attributes.Validation;

// Checking against DB for now, later enhance with team/org management
public class ValidUserId : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var userId = (int) value;
        var dbContext = validationContext.GetService<ApplicationDbContext>();

        // User id can be '0', which means user is not assigned etc
        if (userId == 0)
        {
            return ValidationResult.Success;
        }

        var user = dbContext.Users.FindAsync(userId).Result;

        if (user == null)
        {
            return new ValidationResult("User doesn't exist");
        }

        return ValidationResult.Success;
    }
}