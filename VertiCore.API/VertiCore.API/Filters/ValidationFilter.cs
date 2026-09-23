using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace VertiCore.API.Filters
{
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors
                                .Select(e => e.ErrorMessage)
                                .ToArray() ?? Array.Empty<string>()
                    );

                context.Result = new BadRequestObjectResult(new
                {
                    Message = "Validation failed",
                    Errors = errors
                });
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}