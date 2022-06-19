using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WorkCleverSolution.Controllers;

namespace WorkCleverSolution.Attributes
{
    public class ExceptionHandlerFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var result = new ServiceResult();

            var exception = context.Exception;
            if (exception == null)
            {
                // Should never happen.
                return;
            }
            else if (exception is System.ApplicationException)
            {
                context.HttpContext.Response.StatusCode = 400;
                result.SetMessage(exception.Message).BeFail();
            }
            else if (exception is UnauthorizedAccessException)
            {
                context.HttpContext.Response.StatusCode = 401;
                result.SetMessage("UNAUTHORIZED_REQUEST").BeFail();
            }
            else
            {
                context.HttpContext.Response.StatusCode = 400;
                result.SetMessage(exception.Message).BeFail();
            }

            Console.WriteLine("--------------Exception------------");
            Console.WriteLine(exception.Message);
            Console.WriteLine(exception.StackTrace);
            Console.WriteLine(exception?.InnerException?.Message);

            context.Result = new ObjectResult(result);
        }
    }
}