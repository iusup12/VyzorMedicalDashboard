
using Microsoft.AspNetCore.Mvc;
using Vyzor.Application.DTO.Filters;
using Vyzor.Application.Interfaces;

namespace Vyzor.Web.Controllers;

public class DoctorsController : Controller
{
    private readonly IDoctorService _doctorService;

    public DoctorsController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    public async Task<IActionResult> Index(
        [FromQuery] DoctorCatalogFilterDTO filter,
        CancellationToken cancellationToken)
    {
        filter.PageSize = filter.PageSize <= 0 ? 12 : filter.PageSize;

        var doctors = await _doctorService.GetCatalogPagedAsync(
            filter,
            cancellationToken);

        if (IsAjaxRequest())
        {
            return PartialView("_DoctorResults", doctors);
        }

        return View(doctors);
    }

    [HttpGet]
    public async Task<IActionResult> Details(
        int id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return RedirectToAction(nameof(Index));
        }

        var doctor = await _doctorService.GetDetailsAsync(
            id,
            cancellationToken);

        if (doctor is null)
        {
            return NotFound();
        }

        return View(doctor);
    }

    private bool IsAjaxRequest()
    {
        return string.Equals(
            Request.Headers["X-Requested-With"].ToString(),
            "XMLHttpRequest",
            StringComparison.OrdinalIgnoreCase);
    }
}

