using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Experimentation.Api.Filters
{
    public class CheckModelState : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var msg = BuildModelErrorsString(context.ModelState);
                context.Result = new BadRequestObjectResult(msg);
            }
        }

        private static string BuildModelErrorsString(ModelStateDictionary contextModelState)
        {
            var errors = new StringBuilder($"The model has {contextModelState.ErrorCount} validation errors: ");
            errors.AppendLine("");

            foreach (var item in contextModelState)
            {
                errors.AppendLine($"\t - {item.Key}: {item.Value.Errors.FirstOrDefault()?.ErrorMessage}");
            }

            return errors.ToString();
        }
    }
}