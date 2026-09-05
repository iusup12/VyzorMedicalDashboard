using Microsoft.AspNetCore.Mvc;
using Vyzor.Application.Interfaces;

namespace Vyzor.Web.Controllers;

public class HomeController : Controller
{
    private readonly IDoctorService _doctorService;

    public HomeController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        var doctors = await _doctorService.GetCatalogAsync(
            cancellationToken);

        return View(doctors);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(
        Duration = 0,
        Location = ResponseCacheLocation.None,
        NoStore = true)]
    public IActionResult Error()
    {
        return RedirectToAction(
            "ServerError",
            "Errors");
    }
}