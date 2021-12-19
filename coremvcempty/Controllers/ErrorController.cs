using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coremvcempty.Controllers
{
    public class ErrorController : Controller
    {

        private readonly ILogger logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
        }


        [Route("Error/{statusCode}")]
        public IActionResult NotFoundPage(int statusCode)
        {
            var errorDetails = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = " Could not found the resource";
                    logger.LogError($"There was error processing the resource {errorDetails.OriginalPath}");
                    break;
            }

          

           
            return View();
        }

        [Route("Error")]
        public IActionResult ErrorHandler()
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            ViewBag.Path = exceptionDetails.Path;
            ViewBag.ExceptionMessage = exceptionDetails.Error.Message;
            ViewBag.ErrorMessage = "Exception occured";

            logger.LogError($"Exception occured at {exceptionDetails.Path}");
            return View();
        }
    }

}


