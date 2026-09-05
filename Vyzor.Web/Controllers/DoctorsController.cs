using Microsoft.AspNetCore.Mvc;
using Vyzor.Application.Interfaces;

namespace Vyzor.Web.Controllers;

public class DoctorsController : Controller
{
    private readonly IDoctorService _doctorService;

    public DoctorsController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }


    // =========================================================
    // DOCTORS GRID
    // GET: /Doctors
    // =========================================================

    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        var doctors = await _doctorService.GetCatalogAsync(
            cancellationToken);

        return View(doctors);
    }


    // =========================================================
    // DOCTOR DETAILS
    // GET: /Doctors/Details/1
    // =========================================================

    public async Task<IActionResult> Details(
        int id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return NotFound();
        }


        var doctor = await _doctorService.GetDetailsAsync(
            id,
            cancellationToken);


        if (doctor == null)
        {
            return NotFound();
        }


        return View(doctor);
    }
}