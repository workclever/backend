using System.ComponentModel.DataAnnotations;
using WorkCleverSolution.Data;

namespace WorkCleverSolution.Attributes.Validation;

public class ValidUserId : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var dbContext = validationContext.GetService<ApplicationDbContext>();

        if (value is int singleUserId)
        {
            return ValidateSingleUserId(singleUserId, dbContext);
        }
        if (value is List<int> userIds)
        {
            return ValidateMultipleUserIds(userIds, dbContext);
        }

        return new ValidationResult("Invalid type for UserId. Expected int or List<int>.");
    }

    private ValidationResult ValidateSingleUserId(int userId, ApplicationDbContext dbContext)
    {
        // User id can be '0', which means user is not assigned etc
        if (userId == 0)
        {
            return ValidationResult.Success;
        }
        var user = dbContext.Users.FindAsync(userId).Result;

        if (user == null)
        {
            return new ValidationResult($"User with ID {userId} doesn't exist");
        }

        return ValidationResult.Success;
    }

    private ValidationResult ValidateMultipleUserIds(List<int> userIds, ApplicationDbContext dbContext)
    {
        foreach (var userId in userIds)
        {
            var result = ValidateSingleUserId(userId, dbContext);
            if (result != ValidationResult.Success)
            {
                return result;
            }
        }

        return ValidationResult.Success;
    }
}