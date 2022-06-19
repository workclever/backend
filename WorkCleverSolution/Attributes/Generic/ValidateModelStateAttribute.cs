using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WorkCleverSolution.Controllers;

namespace WorkCleverSolution.Attributes
{
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var validationMessage = context.ModelState
                    .Values
                    .Select(r => r.Errors)
                    .FirstOrDefault()?
                    .Select(r => r.ErrorMessage)
                    .FirstOrDefault();

                var serviceResult = new ServiceResult()
                {
                    Message = validationMessage,
                    Succeed = false
                };

                var objResult = new ObjectResult(serviceResult)
                {
                    StatusCode = 400
                };
                context.Result = objResult;
            }
        }
    }
}