using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace PresentationApi.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState != null && !context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(e => e.Value?.Errors?.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors?.Select(e => e.ErrorMessage).ToArray() ?? new string[0] 
                    );

                var problemDetails = new ValidationProblemDetails(errors)
                {
                    Title = "Validation Error",
                    Status = 400,
                    Detail = "Please refer to the errors property for additional details"
                };

                context.Result = new BadRequestObjectResult(problemDetails);
            }
        }
    }
}
