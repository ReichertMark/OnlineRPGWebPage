using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MMOServer.Controllers
{
  public class ErrorController : Controller
  {
    private readonly ILogger _logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
      _logger = logger;
    }

    [Route("Error/{statusCode}")]
    public IActionResult HttpStatusCodeHandler(int statusCode)
    {
      var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

      switch (statusCode)
      {
        case 404:
          ViewBag.ErrorMessage = "The Resource could not be found (404)";
          _logger.LogWarning($"404  Error occured. Path= {statusCodeResult.OriginalPath}" +
            $"and QueryString = {statusCodeResult.OriginalQueryString}");
          break;
      }

      return View("NotFound");
    }


    [Route("Error")]
    [AllowAnonymous]
    public IActionResult Error()
    {
      var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

      ViewBag.ExceptionPath = exceptionDetails.Path;
      ViewBag.ExceptionMessage = exceptionDetails.Error.Message;
      ViewBag.StackTrace = exceptionDetails.Error.StackTrace;

      _logger.LogError($"The path {exceptionDetails.Path} threw an exception" +
        $"{exceptionDetails.Error}");

      return View("Error");
    }

  }
}
