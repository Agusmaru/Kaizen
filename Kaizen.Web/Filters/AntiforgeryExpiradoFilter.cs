using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Kaizen.Web.Filters;

public sealed class AntiforgeryExpiradoFilter : IAsyncAlwaysRunResultFilter
{
    public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is IAntiforgeryValidationFailedResult)
        {
            var aceptaJson = context.HttpContext.Request.Headers.Accept
                .Any(value => value?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true);

            context.Result = aceptaJson
                ? new JsonResult(new { message = "La sesión de la página venció. Recargala e intentá nuevamente." }) { StatusCode = StatusCodes.Status400BadRequest }
                : new RedirectToActionResult("Ingresar", "Cuenta", new { sesionExpirada = true });
        }

        return next();
    }
}
