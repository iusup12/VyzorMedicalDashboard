using Microsoft.AspNetCore.Mvc;

public class ErrorsController : Controller
{
    [Route("/Error")]
    public IActionResult Error()
    {
        return View("~/Views/Shared/Error.cshtml");
    }
    [Route("/Error/404")]
    public IActionResult NotFound()
    {
        Response.StatusCode = StatusCodes.Status404NotFound;

        return View("~/Views/Shared/404.cshtml");
    }
    [Route("/Error/403")]
    public IActionResult Forbidden()
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;

        return View("~/Views/Shared/403.cshtml");
    }


}