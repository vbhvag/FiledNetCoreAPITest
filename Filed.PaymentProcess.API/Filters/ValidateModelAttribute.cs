using Filed.PaymentProcess.API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Filed.PaymentProcess.API.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var response = new APIResponse { 
                    IsSuccess=false,
                    Message = "One or more validation errors occured"
                };

                foreach (var modelState in context.ModelState)
                {
                    var errorMesages = string.Join("; ", modelState.Value.Errors.Select(e => e.ErrorMessage));
                    response.Errors.Add($"{modelState.Key}: {errorMesages}");
                }

                context.Result = new BadRequestObjectResult(response);
            }
        }
    }
}
